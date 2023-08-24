using Autodesk.Revit.DB;
using CodraftPlugin_DAL;
using CodraftPlugin_Exceptions;
using CodraftPlugin_UIDatabaseWPF;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CodraftPlugin_Updaters.FittingTypes
{
    public class Tee : BaseFitting
    {
        const float radiansToDegrees = 57.295f;
        const float feetToMm = 304.8f;

        public string Nd1 { get; private set; }
        public string Nd2 { get; private set; }
        public string Nd3 { get; private set; }
        public string Hoek { get; private set; }
        public string StrSQL { get; set; }
        public string StrCountSQL { get; set; }

        public Tee(FamilyInstance tee, Document doc, string databaseMapPath, string textFilesMapPath, JObject file)
            : base(tee, doc, databaseMapPath, textFilesMapPath, file)
        {
            this.Nd1 = Math.Round(tee.LookupParameter((string)file["parameters"]["tee"]["property_25"]["revit"]).AsDouble() * feetToMm).ToString();
            this.Nd2 = Math.Round(tee.LookupParameter((string)file["parameters"]["tee"]["property_26"]["revit"]).AsDouble() * feetToMm).ToString();
            this.Nd3 = Math.Round(tee.LookupParameter((string)file["parameters"]["tee"]["property_27"]["revit"]).AsDouble() * feetToMm).ToString();
            this.Hoek = Math.Round(tee.LookupParameter((string)file["parameters"]["tee"]["property_28"]["revit"]).AsDouble() * radiansToDegrees).ToString();

            StrSQL = $"SELECT *" +
                $" FROM {(string)file["parameters"]["tee"]["property_29"]["database"]}" +
                $" WHERE {(string)file["parameters"]["tee"]["property_25"]["database"]} = {this.Nd1}" +
                $" AND {(string)file["parameters"]["tee"]["property_26"]["database"]} = {this.Nd2}" +
                $" AND {(string)file["parameters"]["tee"]["property_27"]["database"]} = {this.Nd3}" +
                $" AND {(string)file["parameters"]["tee"]["property_28"]["database"]} = {this.Hoek};";

            StrCountSQL = $"SELECT COUNT(*)" +
                $" FROM {(string)file["parameters"]["tee"]["property_29"]["database"]}" +
                $" WHERE {(string)file["parameters"]["tee"]["property_25"]["database"]} = {this.Nd1}" +
                $" AND {(string)file["parameters"]["tee"]["property_26"]["database"]} = {this.Nd2}" +
                $" AND {(string)file["parameters"]["tee"]["property_27"]["database"]} = {this.Nd3}" +
                $" AND {(string)file["parameters"]["tee"]["property_28"]["database"]} = {this.Hoek};";
        }

        /// <summary>
        /// Get all params from the database. 
        /// </summary>
        /// <returns>Returns a list with parameters.</returns>
        /// <exception cref="FittingDoesNotExistException">Throws an exception if the the fitting does not exist in the database.</exception>
        public List<object> GetParamsFromDB()
        {
            // Check for multiple rows in database.
            if (FileOperations.LookupTee(StrSQL, StrCountSQL, ConnectionString, out List<object> paramList, parametersConfiguration))
            {
                List<string> parameters = new List<string>() { Nd1, Nd2, Nd3, Hoek, Fi.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString() };

                if (FileOperations.IsFound(parameters, RememberMeFile, out List<string> parameterList))
                {
                    List<object> correctList = new List<object>();

                    correctList.AddRange(parameterList.GetRange(0, 19).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameterList.GetRange(19, 6));

                    return correctList;
                }

                Window w = new UIDatabase(ConnectionString, StrSQL, Doc, Fi, TextFilesMapPath, DatabaseFilePath, RememberMeFile, parameters, parametersConfiguration);
                w.ShowDialog();

                return null;
            }

            // if there are no rows, the fitting does not exist.
            if (paramList.Count == 0)
                throw new FittingDoesNotExistException("Fitting bestaat niet!");

            return paramList;
        }

        /// <summary>
        /// Check if the current parameters are the same as the one from the database.
        /// </summary>
        /// <param name="dbParams"></param>
        /// <returns>Returns true if the parameters are the same.</returns>
        public bool AreParamsTheSame(List<object> dbParams)
        {
            List<object> dbParamsCorrect = ChangeDecimalPoint(dbParams);
            List<object> fittingParams = new List<object>
            {
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_1"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_2"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_3"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_4"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_5"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_6"]["revit"]).AsDouble(), 4),
                Convert.ToDouble(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_7"]["revit"]).AsInteger()),
                Convert.ToDouble(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_8"]["revit"]).AsInteger()),
                Convert.ToDouble(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_9"]["revit"]).AsInteger()),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_10"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_11"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_12"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_13"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_14"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_15"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_16"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_17"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_18"]["revit"]).AsDouble(), 4),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_19"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_20"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_21"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_22"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_23"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_24"]["revit"]).AsString()
            };

            for (int i = 0; i < dbParamsCorrect.Count; i++)
            {
                if (i < 18)
                {
                    if ((double)fittingParams[i] != (double)dbParamsCorrect[i])
                        return false;
                }

                else
                {
                    if (((string)fittingParams[i]).Trim() != ((string)dbParamsCorrect[i]).Trim())
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the fitting was already a wrong, based on the parameters
        /// </summary>
        /// <returns>Returns true if the fitting was already wrong.</returns>
        public bool IsAlreadyWrong()
        {
            if (
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_19"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_20"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_21"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_22"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_23"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_24"]["revit"]).AsString() == "nee" &&

                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_1"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_2"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_3"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_5"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_6"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_4"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_7"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_8"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_9"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_10"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_11"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_12"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_13"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_14"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tee"]["property_15"]["revit"]).AsDouble() == 0
                )
                return true;

            return false;
        }

        /// <summary>
        /// Round all number to the nearest 1/10.000.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private List<object> ChangeDecimalPoint(List<object> parameters)
        {
            List<object> result = new List<object>();

            result.AddRange(parameters.GetRange(0, 18).Select(p => (object)Math.Round((double)p, 4)));
            result.AddRange(parameters.GetRange(18, 6));

            return result;
        }
    }
}
