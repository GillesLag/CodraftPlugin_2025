using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;

namespace CodraftPlugin_DAL
{
    public static class FileOperations
    {
        const float feetToMm = 304.8f;

        /// <summary>
        /// Does a lookup in a an access database with a given query and connectionstring
        /// </summary>
        /// <param name="query">query for the database</param>
        /// <param name="queryCount">query for the database</param>
        /// <param name="connectionString">a string to make an OleDbConnection</param>
        /// <param name="paramList">A list with the parameters form the database</param>
        /// <returns>true if there are more then one record in the database, otherwise false</returns>
        public static bool LookupElbow(string query, string queryCount, string connectionString, out List<object> paramList)
        {
            paramList = new List<object>();
            bool hasMultipleRows;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);
                    OleDbCommand countCommand = new OleDbCommand(queryCount, connection);

                    connection.Open();

                    try
                    {
                        using (OleDbDataReader reader = countCommand.ExecuteReader())
                        {
                            reader.Read();

                            if ((int)reader[0] > 1) return true;

                            hasMultipleRows = false;
                        }

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paramList.Add((double)reader["Buitendiameter_1"] / feetToMm);
                                paramList.Add((double)reader["Buitendiameter_2"] / feetToMm);
                                paramList.Add((double)reader["Center_straal"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_1_type"]);
                                paramList.Add((double)reader["Uiteinde_2_type"]);
                                paramList.Add((double)reader["Uiteinde_1_maat"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_2_maat"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_1_lengte"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_2_lengte"] / feetToMm);
                                paramList.Add((double)reader["Flens_dikte"] / feetToMm);
                                paramList.Add((double)reader["Standaard_hoek"]);
                                paramList.Add(reader["Fabrikant"]);
                                paramList.Add(reader["Type"]);
                                paramList.Add(reader["Materiaal"]);
                                paramList.Add(reader["Productcode"]);
                                paramList.Add(reader["Omschrijving"]);
                                paramList.Add(reader["Beschikbaar"]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("LookupElbow reader error", ex.Message);
                        paramList.Clear();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupElbow connection error", ex.Message);
                return false;
            }

            return hasMultipleRows;
        }

        /// <summary>
        /// Does a lookup in a an access database with a given query and connectionstring
        /// </summary>
        /// <param name="query">query for the database</param>
        /// <param name="queryCount">query for the database</param>
        /// <param name="connectionString">a string to make an OleDbConnection</param>
        /// <param name="paramList">A list with the parameters form the database</param>
        /// <returns>true if there are more then one record in the database, otherwise false</returns>
        public static bool LookupTee(string query, string queryCount, string connectionString, out List<object> paramList)
        {
            paramList = new List<object>();
            bool hasMultipleRows;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);
                    OleDbCommand countCommand = new OleDbCommand(queryCount, connection);

                    connection.Open();

                    try
                    {
                        using (OleDbDataReader reader = countCommand.ExecuteReader())
                        {
                            reader.Read();

                            if ((int)reader[0] > 1) return true;

                            hasMultipleRows = false;
                        }

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paramList.Add((double)reader["Buitendiameter_1"] / feetToMm);
                                paramList.Add((double)reader["Buitendiameter_2"] / feetToMm);
                                paramList.Add((double)reader["Buitendiameter_3"] / feetToMm);
                                paramList.Add((double)reader["Lengte"] / feetToMm);
                                paramList.Add((double)reader["Center_uiteinde_3"] / feetToMm);
                                paramList.Add((double)reader["Center_uiteinde_1"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_1_type"]);
                                paramList.Add((double)reader["Uiteinde_2_type"]);
                                paramList.Add((double)reader["Uiteinde_3_type"]);
                                paramList.Add((double)reader["Uiteinde_1_maat"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_2_maat"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_3_maat"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_1_lengte"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_2_lengte"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_3_lengte"] / feetToMm);
                                paramList.Add((double)reader["Flens_dikte_1"] / feetToMm);
                                paramList.Add((double)reader["Flens_dikte_2"] / feetToMm);
                                paramList.Add((double)reader["Flens_dikte_3"] / feetToMm);
                                paramList.Add(reader["Fabrikant"]);
                                paramList.Add(reader["Type"]);
                                paramList.Add(reader["Materiaal"]);
                                paramList.Add(reader["Productcode"]);
                                paramList.Add(reader["Omschrijving"]);
                                paramList.Add(reader["Beschikbaar"]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("LookupTee reader error", ex.Message);
                        paramList.Clear();
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupTee connection error", ex.Message);
                return false;
            }

            return hasMultipleRows;
        }

        /// <summary>
        /// Does a lookup in an access database with a given query and connctionstring
        /// </summary>
        /// <param name="query">A query for access database</param>
        /// <param name="connectionString">A string to make an OleDbConnection</param>
        /// <returns>A list with object parameters for the transition fitting</returns>
        public static bool LookupTransistion(string query, string queryCount, string connectionString, out List<object> paramList)
        {
            paramList = new List<object>();
            bool hasMultipleRows;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);
                    OleDbCommand countCommand = new OleDbCommand(queryCount, connection);

                    connection.Open();

                    try
                    {
                        using (OleDbDataReader reader = countCommand.ExecuteReader())
                        {
                            reader.Read();

                            if ((int)reader[0] > 1) return true;

                            hasMultipleRows = false;
                        }

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paramList.Add((double)reader["Buitendiameter_1"] / feetToMm);
                                paramList.Add((double)reader["Buitendiameter_2"] / feetToMm);
                                paramList.Add((double)reader["Lengte"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_1_type"]);
                                paramList.Add((double)reader["Uiteinde_2_type"]);
                                paramList.Add((double)reader["Uiteinde_1_maat"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_2_maat"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_1_lengte"] / feetToMm);
                                paramList.Add((double)reader["Uiteinde_2_lengte"] / feetToMm);
                                paramList.Add((double)reader["Flens_dikte_1"] / feetToMm);
                                paramList.Add((double)reader["Flens_dikte_2"] / feetToMm);
                                paramList.Add(reader["Fabrikant"]);
                                paramList.Add(reader["Type"]);
                                paramList.Add(reader["Materiaal"]);
                                paramList.Add(reader["Productcode"]);
                                paramList.Add(reader["Omschrijving"]);
                                paramList.Add(reader["Beschikbaar"]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("LookupTransistion reader error", ex.Message);
                        paramList.Clear();
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupTransistion connection error", ex.Message);
                return false;
            }

            return hasMultipleRows;
        }

        /// <summary>
        /// Does a lookup in an access database with a given query and connectionstring
        /// </summary>
        /// <param name="query"></param>
        /// <param name="queryCount"></param>
        /// <param name="connectionString"></param>
        /// <param name="maxDiameter"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static bool LookupTap(string query, string queryCount, string connectionString, double maxDiameter, out List<object> paramList)
        {
            paramList = new List<object>();
            bool hasMultipleRows;

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);
                    OleDbCommand countCommand = new OleDbCommand(queryCount, connection);

                    connection.Open();

                    try
                    {
                        using (OleDbDataReader reader = countCommand.ExecuteReader())
                        {
                            reader.Read();

                            if ((int)reader[0] > 1) return true;

                            hasMultipleRows = false;
                        }

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                paramList.Add((double)reader["Buitendiameter"] / feetToMm);
                                paramList.Add((double)reader["Lengte"] / feetToMm);
                                paramList.Add(maxDiameter / 2);
                                paramList.Add(reader["Fabrikant"]);
                                paramList.Add(reader["Type"]);
                                paramList.Add(reader["Materiaal"]);
                                paramList.Add(reader["Productcode"]);
                                paramList.Add(reader["Omschrijving"]);
                                paramList.Add(reader["Beschikbaar"]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("LookupTap reader error", ex.Message);
                        paramList.Clear();
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupTap connection error", ex.Message);
                return false;
            }

            return hasMultipleRows;
        }

        /// <summary>
        /// Saves the chosen preference to a txt file.
        /// </summary>
        /// <param name="paramList"></param>
        /// <param name="path"></param>
        /// <param name="callingParams"></param>
        public static void RememberMe(List<object> paramList, string path, List<string> callingParams)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    string parameters = null;

                    for (int i = 0; i < paramList.Count; i++)
                    {
                        if (parameters == null)
                            parameters = paramList[i].ToString();

                        else
                            parameters += ";" + paramList[i].ToString();
                    }

                    writer.WriteLine(string.Join(";", callingParams));
                    writer.WriteLine(parameters);
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("RememberMe Error", ex.Message);
            }
        }

        /// <summary>
        /// Search for fitting params, if fitting is found give parameters in an out keyword, if not found parameters is null.
        /// </summary>
        /// <param name="callingParams"></param>
        /// <param name="path"></param>
        /// <param name="parameters"></param>
        /// <returns>true if the fitting is found ohterwise false</returns>
        public static bool IsFound(List<string> callingParams, string path, out List<string> parameters)
        {
            try
            {
                string call = string.Join(";", callingParams);

                using (StreamReader reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        if (reader.ReadLine() == call)
                        {
                            parameters = reader.ReadLine().Split(';').ToList();
                            return true;
                        }
                    }
                }

                parameters = null;
                return false;
            }

            catch (FileNotFoundException)
            {
                File.Create(path).Close();
                parameters = null;
                return false;
            }

            catch (Exception ex)
            {
                TaskDialog.Show("IsFound error", ex.Message);
                parameters = null;
                return false;
            }
        }

        /// <summary>
        /// Checks in the database that the angel is shortenable for an elbow fitting.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns>True if it is shortenable, otherwise false</returns>
        public static bool IsAngleShortenable(string query, string connectionString)
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);

                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return (double)reader[0] == 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("IsAngleShorenable error", ex.Message);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Checks the database for the correct insulation.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="databaseMapPath"></param>
        /// <returns>A list with the type and thickness of the insulation. if there're no rows it returns null.</returns>
        public static List<object> LookupInsulation(string query, string databaseMapPath)
        {
            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={databaseMapPath}";
            List<object> paramList = new List<object>();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);

                    connection.Open();

                    try
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (!reader.HasRows)
                                return null;

                            while (reader.Read())
                            {
                                paramList.Add((string)reader["Isolatie_materiaal"]);
                                paramList.Add((double)reader["Isolatie_dikte"] / feetToMm);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("LookupInsulation reader error", ex.Message);
                        paramList.Clear();
                    }

                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupInsulation connection error", ex.Message);
            }

            return paramList;
        }

        /// <summary>
        /// Get all the materials from the database.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns>a list of strings with the names of the materials</returns>
        public static List<string> GetMaterials(string query, string connectionString)
        {
            List<string> materialList = new List<string>();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);

                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string mat = (string)reader[0];

                            if (!materialList.Contains(mat))
                                materialList.Add(mat);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupInsulation connection error", ex.Message);
            }

            return materialList;
        }

        /// <summary>
        /// Get all the schedules/types(ProducerName) from the database.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns>A list of strings with the schedule names.</returns>
        public static List<string> GetSchedules(string query, string connectionString)
        {
            List<string> scheduleList = new List<string>();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);

                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string schedule = (string)reader[0];

                            if (!scheduleList.Contains(schedule))
                                scheduleList.Add(schedule);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupInsulation connection error", ex.Message);
            }

            return scheduleList;
        }

        /// <summary>
        /// Get all the MEPSizes from the database.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns>A list of MEPSizes</returns>
        public static List<MEPSize> GetMepSizes(string query, string connectionString)
        {
            List<MEPSize> mepSizeList = new List<MEPSize>();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);

                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            double dn = (double)reader[0] / feetToMm;
                            double inner = (double)reader[1] / feetToMm;
                            double outer = (double)reader[2] /feetToMm;

                            MEPSize size = new MEPSize(dn, inner, outer, true, true);

                            if (!mepSizeList.Contains(size))
                                mepSizeList.Add(size);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupInsulation connection error", ex.Message);
            }

            return mepSizeList;
        }

        /// <summary>
        /// Get the total amount of segments
        /// </summary>
        /// <param name="countQuery"></param>
        /// <param name="connectionString"></param>
        /// <returns>An int indicating how many segments there are.</returns>
        public static List<List<object>> GetSegmentsAndSizeList(string joinQuery, string connectionString)
        {
            List<List<object>> segmentsAndSizeList = new List<List<object>>();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(joinQuery, connection);

                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        List<object> segments = new List<object>();

                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                segments.Add(reader[i]);
                            }

                            segmentsAndSizeList.Add(segments);
                            segments = new List<object>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupInsulation connection error", ex.Message);
            }

            return segmentsAndSizeList;
        }

        /// <summary>
        /// Get the insulation materials from the database.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns>A list of strings with the name of the insulation material</returns>
        public static List<string> GetInsulationMaterials(string query, string connectionString)
        {
            List<string> insulationMaterialsList = new List<string>();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);

                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            insulationMaterialsList.Add((string)reader[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupInsulation connection error", ex.Message);
            }

            return insulationMaterialsList;
        }

        /// <summary>
        /// Get all the systemTypes from the database
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns>A table with all the info of the systemTypes</returns>
        public static List<List<string>> GetSystemTypes(string query, string connectionString)
        {
            List<List<string>> systemTypesList = new List<List<string>>();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand commmand = new OleDbCommand(query, connection);

                    connection.Open();

                    using (OleDbDataReader reader = commmand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            List<string> stringList = new List<string>();

                            for (int i = 1; i < reader.FieldCount; i++)
                            {
                                stringList.Add((string)reader[i]);
                            }

                            systemTypesList.Add(stringList);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupInsulation connection error", ex.Message);
            }

            return systemTypesList;
        }

        /// <summary>
        /// Get all the pipetypes and segments from the database
        /// </summary>
        /// <param name="query"></param>
        /// <param name="connectionString"></param>
        /// <returns>A list of list of strings with data</returns>
        public static List<List<object>> GetPipeTypes(string query, string connectionString)
        {
            List<List<object>> ongesorteerdeList = new List<List<object>>();
            List<List<object>> gesorteerdeList = new List<List<object>>();

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);

                    connection.Open();

                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                        List<object> data = new List<object>();
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var test = reader[i];
                                data.Add(reader[i]);
                            }

                            ongesorteerdeList.Add(data);
                            data = new List<object>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("LookupInsulation connection error", ex.Message);
            }

            for (int i = 0; i < ongesorteerdeList.Count; i++)
            {
                string typeNaam = (string)ongesorteerdeList[i][1];

                for (int j = 0; j < ongesorteerdeList.Count; j++)
                {
                    if (typeNaam == (string)ongesorteerdeList[j][1] && !gesorteerdeList.Contains(ongesorteerdeList[j]))
                    {
                        gesorteerdeList.Add(ongesorteerdeList[j]);
                    }
                }
            }

            return gesorteerdeList;
        }

        /// <summary>
        /// This wil give you all the insulationdata to update all the insulation in your project
        /// </summary>
        /// <param name="SQLstring"></param>
        /// <param name="connectionString"></param>
        /// <returns>a crazy dictionary that probaly should be refactored some time</returns>
        public static Dictionary<string, Dictionary<string, Dictionary<double, List<double>>>> IsolatieData(string SQLstring, string connectionString)
        {
            Dictionary<string,
                Dictionary<string,
                Dictionary<double,
                List<double>>>> data = new Dictionary<string,
                                                Dictionary<string,
                                                Dictionary<double,
                                                List<double>>>>();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(SQLstring, conn);

                conn.Open();

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string med = (string)reader["Medium"];
                        string isolMat = (string)reader["Isolatie_materiaal"];
                        double isolDikte = (double)reader["Isolatie_dikte"];
                        double nd = (double)reader["Nominale_diameter"];

                        
                        if (!data.ContainsKey(med))
                        {
                            data.Add(med, new Dictionary<string, Dictionary<double, List<double>>>());
                            data[med].Add(isolMat, new Dictionary<double, List<double>>());
                            data[med][isolMat].Add(isolDikte, new List<double>());
                            data[med][isolMat][isolDikte].Add(nd);

                            continue;
                        }

                        if (!data[med].ContainsKey(isolMat))
                        {
                            data[med].Add(isolMat, new Dictionary<double, List<double>>());
                            data[med][isolMat].Add(isolDikte, new List<double>());
                            data[med][isolMat][isolDikte].Add(nd);

                            continue;
                        }

                        if (!data[med][isolMat].ContainsKey(isolDikte))
                        {
                            data[med][isolMat].Add(isolDikte, new List<double>());
                            data[med][isolMat][isolDikte].Add(nd);

                            continue;
                        }

                        if (!data[med][isolMat][isolDikte].Contains(nd))
                            data[med][isolMat][isolDikte].Add(nd);
                    }
                }
            }

            return data;
        }
    }
}
