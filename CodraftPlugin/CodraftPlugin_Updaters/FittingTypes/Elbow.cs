using Autodesk.Revit.DB;
using CodraftPlugin_DAL;
using CodraftPlugin_Exceptions;
using CodraftPlugin_UIDatabaseWPF;
using CodraftPlugin_UIDatabaseWPF.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;


namespace CodraftPlugin_Updaters.FittingTypes
{
    public class Elbow : BaseFitting
    {
        const float radiansToDegrees = 57.295f;
        const float feetToMm = 304.8f;

        public string Nd1 { get; private set; }
        public string Nd2 { get; private set; }
        public string HoekTolerantie { get; set; }
        public string HoekStandaard { get; set; } = "45";
        public string Hoek { get; set; }
        public string StrSQL { get; set; }
        public string StrCountSQL { get; set; }
        public string StrHoekInkortbaarSQL { get; set; }

        private double hoekGetekent;

        public Elbow(FamilyInstance elbow, Document doc, string databaseMapPath, string textFilesMapPath, JObject file)
            : base(elbow, doc, databaseMapPath, textFilesMapPath, file)
        {
            hoekGetekent = elbow.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_19"]["revit"]).AsDouble() * radiansToDegrees;
            this.Nd1 = Math.Round(elbow.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_20"]["revit"]).AsDouble() * feetToMm).ToString();
            this.Nd2 = Math.Round(elbow.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_21"]["revit"]).AsDouble() * feetToMm).ToString();
            this.HoekTolerantie = Math.Round(elbow.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_18"]["revit"]).AsDouble() * radiansToDegrees).ToString();
            this.Hoek = Math.Round(hoekGetekent).ToString();

            if (double.Parse(this.Hoek) > 45) this.HoekStandaard = "90";

            StrSQL = $"SELECT *" +
                $" FROM {(string)parametersConfiguration["parameters"]["elbow"]["property_22"]["database"]}" +
                $" WHERE {(string)parametersConfiguration["parameters"]["elbow"]["property_20"]["database"]} = {this.Nd1}" +
                $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_21"]["database"]} = {this.Nd2}" +
                $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_17"]["database"]} = {this.HoekTolerantie};";

            StrCountSQL = $"SELECT COUNT(*)" +
                $" FROM {(string)parametersConfiguration["parameters"]["elbow"]["property_22"]["database"]}" +
                $" WHERE {(string)parametersConfiguration["parameters"]["elbow"]["property_20"]["database"]} = {this.Nd1}" +
                $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_21"]["database"]} = {this.Nd2}" +
                $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_17"]["database"]} = {this.HoekTolerantie};";

            StrHoekInkortbaarSQL = $"SELECT {(string)parametersConfiguration["parameters"]["elbow"]["property_23"]["database"]}" +
                $" FROM {(string)parametersConfiguration["parameters"]["elbow"]["property_22"]["database"]}" +
                $" WHERE {(string)parametersConfiguration["parameters"]["elbow"]["property_20"]["database"]} = {this.Nd1}" +
                $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_21"]["database"]} = {this.Nd2}";
        }

        /// <summary>
        /// Get all params from the database. 
        /// </summary>
        /// <returns>Returns a list with parameters.</returns>
        /// <exception cref="FittingDoesNotExistException">Throws an exception if the the fitting does not exist in the database.</exception>
        public List<object> GetParamsFromDB()
        {
            if (FileOperations.IsAngleShortenable(StrHoekInkortbaarSQL, ConnectionString))
            {
                string StrStandaardHoekSql = $"SELECT {(string)parametersConfiguration["parameters"]["elbow"]["property_17"]["database"]}" +
                $" FROM {(string)parametersConfiguration["parameters"]["elbow"]["property_22"]["database"]}" +
                $" WHERE {(string)parametersConfiguration["parameters"]["elbow"]["property_20"]["database"]} = {this.Nd1}" +
                $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_21"]["database"]} = {this.Nd2}" +
                $" ORDER BY {(string)parametersConfiguration["parameters"]["elbow"]["property_17"]["database"]}";

                this.HoekStandaard = FileOperations.GetHoekStandaard(ConnectionString, StrStandaardHoekSql, hoekGetekent);

                StrSQL = $"SELECT *" +
                    $" FROM {(string)parametersConfiguration["parameters"]["elbow"]["property_22"]["database"]}" +
                    $" WHERE {(string)parametersConfiguration["parameters"]["elbow"]["property_20"]["database"]} = {this.Nd1}" +
                    $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_21"]["database"]} = {this.Nd2}" +
                    $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_17"]["database"]} = {this.HoekStandaard};";

                StrCountSQL = $"SELECT COUNT(*)" +
                    $" FROM {(string)parametersConfiguration["parameters"]["elbow"]["property_22"]["database"]}" +
                    $" WHERE {(string)parametersConfiguration["parameters"]["elbow"]["property_20"]["database"]} = {this.Nd1}" +
                    $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_21"]["database"]} = {this.Nd2}" +
                    $" AND {(string)parametersConfiguration["parameters"]["elbow"]["property_17"]["database"]} = {this.HoekStandaard};";
            }

            // Check for multiple rows in database.
            if (FileOperations.LookupElbow(StrSQL, StrCountSQL, ConnectionString, out List<object> paramList, parametersConfiguration))
            {
                List<string> parameters = new List<string>() { Nd1, Nd2, HoekTolerantie, Fi.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString() };

                if (FileOperations.IsFound(parameters, RememberMeFile, out List<string> parameterList))
                {
                    List<object> correctList = new List<object>();

                    correctList.AddRange(parameterList.GetRange(0, 11).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameterList.GetRange(11, 6));

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
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_1"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_2"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_3"]["revit"]).AsDouble(), 4),
                Convert.ToDouble(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_4"]["revit"]).AsInteger()),
                Convert.ToDouble(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_5"]["revit"]).AsInteger()),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_6"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_7"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_8"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_9"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_10"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_17"]["revit"]).AsDouble(), 4),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_11"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_12"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_13"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_14"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_15"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_16"]["revit"]).AsString()
            };

            for (int i = 0; i < dbParamsCorrect.Count; i++)
            {
                if (i < 11)
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
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_11"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_12"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_13"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_14"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_15"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_16"]["revit"]).AsString() == "nee" &&

                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_1"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_2"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_3"]["revit"]).AsValueString() == "30" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_4"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_5"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_6"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_7"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_8"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["elbow"]["property_9"]["revit"]).AsDouble() == 0
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

            result.AddRange(parameters.GetRange(0, 11).Select(p => (object)Math.Round((double)p, 4)));
            result.AddRange(parameters.GetRange(11, 6));

            return result;
        }

    }
}
