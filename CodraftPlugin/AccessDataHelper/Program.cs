using System.Data.OleDb;
using System.Xml;
using Newtonsoft.Json;

namespace AccessDataHelper;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("niet alle argumenten zijn ingegeven");
            return;
        }

        string connectionString = args[0];
        string query = args[1];

        try
        {
            var results = new List<Dictionary<string, object>>();

            using (var connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                using (var command = new OleDbCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        results.Add(row);
                    }
                }
            }

            // Serialize results to JSON
            string json = JsonConvert.SerializeObject(results);
            Console.WriteLine(json); // Output results
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }
}
