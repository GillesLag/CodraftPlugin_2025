using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using System.Security.Cryptography;

namespace CodraftPlugin_DAL
{
    public static class FileOperationsPipeAccessories
    {
        private const double feetToMm = 304.8;

        public static bool LookupBalanceValve(string query, string queryCount, string connectionString, out List<object> parameters)
        {
            parameters = new List<object>();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                OleDbCommand countCommand = new OleDbCommand(queryCount, connection);

                connection.Open();

                using (OleDbDataReader reader = countCommand.ExecuteReader())
                {
                    reader.Read();
                    int count = (int)reader[0];
                    if (count == 0) return false;
                    if ((int)reader[0] > 1) return true;
                }

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    reader.Read();

                    parameters.Add(Math.Round((double)reader["Lengte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["PipeOd"] / feetToMm, 4));
                    parameters.Add((int)reader["Uiteinde_1_type"]);
                    parameters.Add((int)reader["Uiteinde_2_type"]);
                    parameters.Add(Math.Round((double)reader["Uiteinde_1_maat"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Uiteinde_2_maat"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["L1"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["L2"] / feetToMm, 4));
                    parameters.Add(reader["Manufacturer"]);
                    parameters.Add(reader["Type"]);
                    parameters.Add(reader["Material"]);
                    parameters.Add(reader["Product Code"]);
                    parameters.Add(reader["Omschrijving"]);
                    parameters.Add(reader["Beschikbaar"]);
                }
            }

            return false;
        }

        public static bool LookupButterflyValve(string query, string queryCount, string connectionString, out List<object> parameters)
        {
            parameters = new List<object>();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                OleDbCommand countCommand = new OleDbCommand(queryCount, connection);

                connection.Open();

                using (OleDbDataReader reader = countCommand.ExecuteReader())
                {
                    reader.Read();
                    int count = (int)reader[0];
                    if (count == 0) return false;
                    if ((int)reader[0] > 1) return true;
                }

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    reader.Read();

                    parameters.Add(Math.Round((double)reader["CompOD"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["CompLen"] / feetToMm, 4));
                    parameters.Add(Math.Round((int)reader["D1"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["b"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["b"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["b"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["c"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["d"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Thickness"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["BladeDiameter"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["c"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["d"] / feetToMm, 4));
                    parameters.Add(reader["Manufacturer"]);
                    parameters.Add(reader["Type"]);
                    parameters.Add(reader["Material"]);
                    parameters.Add(reader["Product Code"]);
                    parameters.Add(reader["Omschrijving"]);
                    parameters.Add(reader["Beschikbaar"]);
                }
            }

            return false;
        }

        public static bool LookupStraightValve(string query, string queryCount, string connectionString, out List<object> parameters)
        {
            parameters = new List<object>();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                OleDbCommand countCommand = new OleDbCommand(queryCount, connection);

                connection.Open();

                using (OleDbDataReader reader = countCommand.ExecuteReader())
                {
                    reader.Read();
                    int count = (int)reader[0];
                    if (count == 0) return false;
                    if ((int)reader[0] > 1) return true;
                }

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    reader.Read();

                    parameters.Add(Math.Round((double)reader["Lengte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Hendel_lengte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Hendel_breedte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Hendel_hoogte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Motor_lengte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Motor_breedte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Motor_hoogte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Wormwiel_straal"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Wormwiel_staaf_straal"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Operator_hoogte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Vlinderhendel_diameter"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["PipeOD"] / feetToMm, 4));
                    parameters.Add((int)reader["Uiteinde_1_type"]);
                    parameters.Add((int)reader["Uiteinde_2_type"]);
                    parameters.Add(Math.Round((double)reader["Uiteinde_1_maat"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Uiteinde_2_maat"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["L1"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["L2"] / feetToMm, 4));
                    parameters.Add(reader["Manufacturer"]);
                    parameters.Add(reader["Type"]);
                    parameters.Add(reader["Material"]);
                    parameters.Add(reader["Product Code"]);
                    parameters.Add(reader["Omschrijving"]);
                    parameters.Add(reader["Beschikbaar"]);
                }
            }

            return false;
        }

        public static bool LookupStrainer(string query, string queryCount, string connectionString, out List<object> parameters)
        {
            parameters = new List<object>();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                OleDbCommand countCommand = new OleDbCommand(queryCount, connection);

                connection.Open();

                using (OleDbDataReader reader = countCommand.ExecuteReader())
                {
                    reader.Read();
                    int count = (int)reader[0];
                    if (count == 0) return false;
                    if ((int)reader[0] > 1) return true;
                }

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    reader.Read();

                    parameters.Add(Math.Round((double)reader["PipeOD"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Height"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["CompLen"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["BranchOffset"] / feetToMm, 4));
                    parameters.Add((int)reader["Uiteinde_1_type"]);
                    parameters.Add((int)reader["Uiteinde_2_type"]);
                    parameters.Add(Math.Round((double)reader["L1"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["L2"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Uiteinde_1_maat"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Uiteinde_2_maat"] / feetToMm, 4));
                    parameters.Add(reader["Manufacturer"]);
                    parameters.Add(reader["Type"]);
                    parameters.Add(reader["Material"]);
                    parameters.Add(reader["Product Code"]);
                    parameters.Add(reader["Omschrijving"]);
                    parameters.Add(reader["Beschikbaar"]);
                    parameters.Add(reader["Maat_annotatie"]);
                }
            }

            return false;
        }

        public static bool LookupThreeWayValve(string query, string queryCount, string connectionString, out List<object> parameters)
        {
            parameters = new List<object>();
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                OleDbCommand command = new OleDbCommand(query, connection);
                OleDbCommand countCommand = new OleDbCommand(queryCount, connection);

                connection.Open();

                using (OleDbDataReader reader = countCommand.ExecuteReader())
                {
                    reader.Read();
                    int count = (int)reader[0];
                    if (count == 0) return false;
                    if ((int)reader[0] > 1) return true;
                }

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    reader.Read();

                    parameters.Add(Math.Round((double)reader["PipeOD1"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Lengte"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Lengte_3"] / feetToMm, 4));
                    parameters.Add((int)reader["Uiteinde_1_type"]);
                    parameters.Add((int)reader["Uiteinde_2_type"]);
                    parameters.Add((int)reader["Uiteinde_3_type"]);
                    parameters.Add(Math.Round((double)reader["L1"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["L2"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["L3"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Uiteinde_1_maat"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Uiteinde_2_maat"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["Uiteinde_3_maat"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["a"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["d"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["e"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["b"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["c"] / feetToMm, 4));
                    parameters.Add(Math.Round((double)reader["d"] / feetToMm, 4));
                    parameters.Add(reader["Manufacturer"]);
                    parameters.Add(reader["Type"]);
                    parameters.Add(reader["Material"]);
                    parameters.Add(reader["Product Code"]);
                    parameters.Add(reader["Omschrijving"]);
                    parameters.Add(reader["Beschikbaar"]);
                    //parameters.Add(reader["Maat_annotatie"]);
                }
            }

            return false;
        }
    }
}
