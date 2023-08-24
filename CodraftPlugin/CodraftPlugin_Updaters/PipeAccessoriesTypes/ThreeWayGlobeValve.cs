using Autodesk.Revit.DB;
using CodraftPlugin_DAL;
using CodraftPlugin_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodraftPlugin_PipeAccessoriesWPF;
using Newtonsoft.Json.Linq;

namespace CodraftPlugin_Updaters.PipeAccessoriesTypes
{
    public class ThreeWayGlobeValve : BaseAccessory
    {
        public ThreeWayGlobeValve(FamilyInstance accessory, Document doc, string databaseMapPath, JObject file) : base(accessory, doc, databaseMapPath, file)
        {
            this.Query = $"SELECT * " +
                $"FROM {(string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_25"]["database"]} " +
                $"WHERE {(string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_19"]["database"]} = \"{this.Fabrikant}\" " +
                $"AND {(string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_20"]["database"]} = \"{this.Type}\" " +
                $"AND {(string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_26"]["database"]} = {this.Dn}";

            this.QueryCount = $"SELECT COUNT(*) " +
                $"FROM {(string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_25"]["database"]} " +
                $"WHERE {(string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_19"]["database"]} = \"{this.Fabrikant}\" " +
                $"AND {(string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_20"]["database"]} = \"{this.Type}\" " +
                $"AND {(string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_26"]["database"]} = {this.Dn}";
        }

        public override bool? GetParams()
        {
            List<object> parametersList;

            if (FileOperationsPipeAccessories.LookupThreeWayValve(Query, QueryCount, ConnectionString, out parametersList))
            {
                if (FileOperations.IsFound(CallingParams, RememberMeFilePath, out List<string> parameters))
                {
                    List<object> correctList = new List<object>();

                    correctList.AddRange(parameters.GetRange(0, 3).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameters.GetRange(3, 3).Select(x => (object)int.Parse(x)));
                    correctList.AddRange(parameters.GetRange(6, 12).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameters.GetRange(18, 6));

                    this.DatabaseParameters = correctList;
                    return true;
                }

                string typeName = this.ToString();
                string name = typeName.Substring(typeName.LastIndexOf('.') + 1);
                MainWindow accessoryWindow = new MainWindow(PipeAccessory, name, ConnectionString, Query, DatabaseFilePath, CallingParams, parameterConfiguration);
                accessoryWindow.ShowDialog();

                if (accessoryWindow.hasChosenAccessory)
                    return null;

                return false;
            }

            if (!parametersList.Any()) return false;

            this.DatabaseParameters = parametersList;
            return true;
        }

        public override bool ParametersAreTheSame()
        {
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Buitendiameter").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Lengte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Lengte_3").AsDouble(), 4));
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("Uiteinde_1_type").AsInteger());
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("Uiteinde_2_type").AsInteger());
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("Uiteinde_3_type").AsInteger());
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Uiteinde_1_maat").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Uiteinde_2_maat").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Uiteinde_3_maat").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Uiteinde_1_lengte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Uiteinde_2_lengte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Uiteinde_3_lengte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Motor_lengte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Motor_breedte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Motor_hoogte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Hoogte_operator").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Wormwiel_diameter").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Wormwiel_lengte").AsDouble(), 4));
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("COD_Fabrikant").AsString());
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("COD_Type").AsString());
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("COD_Materiaal").AsString());
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("COD_Productcode").AsString());
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("COD_Omschrijving").AsString());
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("COD_Beschikbaar").AsString());

            return ElementSettings.CompareParameters(this.RevitParameters, this.DatabaseParameters);
        }

        public override void SetWrongValues()
        {
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_1"]["revit"]).Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_2"]["revit"]).Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_3"]["revit"]).Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_4"]["revit"]).Set(0);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_5"]["revit"]).Set(0);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_6"]["revit"]).Set(0);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_7"]["revit"]).Set(0);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_8"]["revit"]).Set(0);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_9"]["revit"]).Set(0);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_10"]["revit"]).Set(0);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_11"]["revit"]).Set(0);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_12"]["revit"]).Set(0);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_13"]["revit"]).Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_14"]["revit"]).Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_15"]["revit"]).Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_16"]["revit"]).Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_17"]["revit"]).Set(15 / feetToMm);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_18"]["revit"]).Set(20 / feetToMm);
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_19"]["revit"]).Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_20"]["revit"]).Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_21"]["revit"]).Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_22"]["revit"]).Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_23"]["revit"]).Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter((string)parameterConfiguration["parameters"]["threewayGlobeValve"]["property_24"]["revit"]).Set("BESTAAT NIET!");
        }
    }
}
