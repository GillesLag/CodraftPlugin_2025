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
    public class Transition : BaseFitting
    {
        const float radiansToDegrees = 57.295f;
        const float feetToMm = 304.8f;

        public string Nd1 { get; private set; }
        public string Nd2 { get; private set; }
        public int Excentrisch { get; set; }
        public string StrSQL { get; set; }
        public string StrCountSQL { get; set; }
        public bool switchNds { get; set; }

        public Transition(FamilyInstance transistion, Document doc, string databaseMapPath, string textFilesMapPath, JObject file)
            : base(transistion, doc, databaseMapPath, textFilesMapPath, file)
        {
            double nd1 = Math.Round(transistion.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_18"]["revit"]).AsDouble() * feetToMm);
            double nd2 = Math.Round(transistion.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_19"]["revit"]).AsDouble() * feetToMm);

            if (nd1 > nd2)
            {
                this.Nd1 = nd1.ToString();
                this.Nd2 = nd2.ToString();
                switchNds = false;
            }
            else
            {
                this.Nd1 = nd2.ToString();
                this.Nd2 = nd1.ToString();
                switchNds = true;
            }

            this.Excentrisch = transistion.LookupParameter((string)file["parameters"]["transistion"]["property_20"]["revit"]).AsInteger();

            StrSQL = $"SELECT *" +
                $" FROM {(string)parametersConfiguration["parameters"]["transistion"]["property_23"]["database"]}" +
                $" WHERE {(string)parametersConfiguration["parameters"]["transistion"]["property_18"]["database"]} = {this.Nd1}" +
                $" AND {(string)parametersConfiguration["parameters"]["transistion"]["property_19"]["database"]} = {this.Nd2}" +
                $" AND {(string)parametersConfiguration["parameters"]["transistion"]["property_20"]["database"]} = {this.Excentrisch};";

            StrCountSQL = $"SELECT COUNT(*)" +
                $" FROM {(string)parametersConfiguration["parameters"]["transistion"]["property_23"]["database"]}" +
                $" WHERE {(string)parametersConfiguration["parameters"]["transistion"]["property_18"]["database"]} = {this.Nd1}" +
                $" AND {(string)parametersConfiguration["parameters"]["transistion"]["property_19"]["database"]} = {this.Nd2}" +
                $" AND {(string)parametersConfiguration["parameters"]["transistion"]["property_20"]["database"]} = {this.Excentrisch};";

        }

        public List<object> GetParamsFromDB()
        {
            // Check for multiple rows in database.
            if (FileOperations.LookupTransistion(StrSQL, StrCountSQL, ConnectionString, out List<object> paramList, parametersConfiguration))
            {
                List<string> parameters = new List<string>() { Nd1, Nd2, Excentrisch.ToString(), Fi.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString() };

                if (FileOperations.IsFound(parameters, RememberMeFile, out List<string> parameterList))
                {
                    List<object> correctList = new List<object>();

                    var nlBE = new System.Globalization.CultureInfo("nl-BE");
                    nlBE.NumberFormat.NumberDecimalSeparator = ",";
                    nlBE.NumberFormat.NumberGroupSeparator = ".";
                    System.Threading.Thread.CurrentThread.CurrentCulture = nlBE;

                    correctList.AddRange(parameterList.GetRange(0, 11).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameterList.GetRange(11, 6));

                    return correctList;
                }

                Window w = new UIDatabase(ConnectionString, StrSQL, Doc, Fi, TextFilesMapPath, DatabaseFilePath, RememberMeFile, parameters, parametersConfiguration, switchNds, Excentrisch);
                w.ShowDialog();

                return null;
            }

            // if there are no rows, throw an error.
            if (paramList.Count == 0)
                throw new FittingDoesNotExistException("Fitting bestaat niet!");

            return paramList;
        }

        public bool AreParamsTheSame(List<object> dbParams, bool switchNds)
        {
            List<object> dbParamsCorrect = ChangeDecimalPoint(dbParams);
            List<object> fittingParams = new List<object>();

            if (!switchNds)
            {
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_1"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_2"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_3"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Convert.ToDouble(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_4"]["revit"]).AsInteger()));
                fittingParams.Add(Convert.ToDouble(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_5"]["revit"]).AsInteger()));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_6"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_7"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_8"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_9"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_10"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_11"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_12"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_13"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_14"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_15"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_16"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_17"]["revit"]).AsString());
            }
            else
            {
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_2"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_1"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_3"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Convert.ToDouble(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_5"]["revit"]).AsInteger()));
                fittingParams.Add(Convert.ToDouble(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_4"]["revit"]).AsInteger()));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_7"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_6"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_9"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_8"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_11"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_10"]["revit"]).AsDouble(), 4));
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_12"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_13"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_14"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_15"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_16"]["revit"]).AsString());
                fittingParams.Add(Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_17"]["revit"]).AsString());
            }


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

        public bool IsAlreadyWrong()
        {
            if (
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_12"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_13"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_14"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_15"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_16"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_17"]["revit"]).AsString() == "nee" &&

                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_1"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_2"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_3"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_4"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_5"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_6"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_7"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_8"]["revit"]).AsDouble() == 0 &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["transistion"]["property_9"]["revit"]).AsDouble() == 0
                )
                return true;

            return false;
        }

        private List<object> ChangeDecimalPoint(List<object> parameters)
        {
            List<object> result = new List<object>();

            result.AddRange(parameters.GetRange(0, 11).Select(p => (object)Math.Round((double)p, 4)));
            result.AddRange(parameters.GetRange(11, 6));

            return result;
        }
    }
}
