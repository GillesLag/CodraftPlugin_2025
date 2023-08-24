using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
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
    public class Tap : BaseFitting
    {
        const float feetToMm = 304.8f;

        public string Nd1 { get; set; }
        public string StrSQL { get; set; }
        public string StrCountSQL { get; set; }
        public double MaxDiameter { get; set; }

        public Tap(FamilyInstance tap, Document doc, string databaseMapPath, string textFilesMapPath, JObject file)
            : base(tap, doc, databaseMapPath, textFilesMapPath, file)
        {
            this.Nd1 = Math.Round(tap.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_10"]["revit"]).AsDouble() * feetToMm).ToString();

            var connectors = tap.MEPModel.ConnectorManager.Connectors.Cast<IConnector>();

            List<double> diameters = new List<double>();

            foreach (var e in connectors)
            {
                var con = e as Connector;
                var refs = con.AllRefs;

                foreach (Connector c in refs)
                {
                    if (c.Owner is Pipe p)
                    {
                        diameters.Add(p.Diameter);
                    }
                }
            }

            this.MaxDiameter = diameters.Max();

            this.StrSQL = $"SELECT *" +
                $" FROM {(string)parametersConfiguration["parameters"]["tap"]["property_12"]["database"]}" +
                $" WHERE {(string)parametersConfiguration["parameters"]["tap"]["property_10"]["database"]} = {this.Nd1};";

            this.StrCountSQL = $"SELECT COUNT(*)" +
                $" FROM {(string)parametersConfiguration["parameters"]["tap"]["property_12"]["database"]}" +
                $" WHERE {(string)parametersConfiguration["parameters"]["tap"]["property_10"]["database"]} = {this.Nd1};";
        }

        public List<object> GetParamsFromDB()
        {
            // Check for multiple rows in database.
            if (FileOperations.LookupTap(StrSQL, StrCountSQL, ConnectionString, MaxDiameter, out List<object> paramList, parametersConfiguration))
            {
                List<string> parameters = new List<string>() { Nd1, Fi.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString() };

                if (FileOperations.IsFound(parameters, RememberMeFile, out List<string> parameterList))
                {
                    List<object> correctList = new List<object>();

                    correctList.AddRange(parameterList.GetRange(0, 3).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameterList.GetRange(3, 6));

                    return correctList;
                }

                Window w = new UIDatabase(ConnectionString, StrSQL, Doc, Fi, TextFilesMapPath, DatabaseFilePath, RememberMeFile, parameters, parametersConfiguration, false, 0 , MaxDiameter);
                w.ShowDialog();

                return null;
            }

            // if there are no rows, throw an error.
            if (paramList.Count == 0)
                throw new FittingDoesNotExistException("Fitting bestaat niet!");

            return paramList;
        }

        public bool AreParamsTheSame(List<object> dbParams)
        {
            List<object> dbParamsCorrect = ChangeDecimalPoint(dbParams);
            List<object> fittingParams = new List<object>
            {
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_1"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_2"]["revit"]).AsDouble(), 4),
                Math.Round(Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_3"]["revit"]).AsDouble(), 4),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_4"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_5"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_6"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_7"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_8"]["revit"]).AsString(),
                Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_9"]["revit"]).AsString()
            };

            for (int i = 0; i < dbParamsCorrect.Count; i++)
            {
                if (i < 2)
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
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_4"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_5"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_6"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_7"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_8"]["revit"]).AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_9"]["revit"]).AsString() == "nee" &&

                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_1"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_2"]["revit"]).AsValueString() == "15" &&
                    Fi.LookupParameter((string)parametersConfiguration["parameters"]["tap"]["property_3"]["revit"]).AsValueString() == "15"
                )
                return true;

            return false;
        }

        private List<object> ChangeDecimalPoint(List<object> parameters)
        {
            List<object> result = new List<object>();

            result.AddRange(parameters.GetRange(0, 3).Select(p => (object)Math.Round((double)p, 4)));
            result.AddRange(parameters.GetRange(3, 6));

            return result;
        }
    }
}
