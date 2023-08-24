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
    public class ButterflyValve : BaseAccessory
    {
        public ButterflyValve(FamilyInstance accessory, Document doc, string databaseMapPath, JObject file) : base(accessory, doc, databaseMapPath, file)
        {
            this.Query = $"SELECT * " +
                $"FROM {(string)parameterConfiguration["parameters"]["butterflyValve"]["property_19"]["database"]} " +
                $"WHERE {(string)parameterConfiguration["parameters"]["butterflyValve"]["property_13"]["database"]} = \"{this.Fabrikant}\" " +
                $"AND {(string)parameterConfiguration["parameters"]["butterflyValve"]["property_14"]["database"]} = \"{this.Type}\" " +
                $"AND {(string)parameterConfiguration["parameters"]["butterflyValve"]["property_3"]["database"]} = {this.Dn}";

            this.QueryCount = $"SELECT COUNT(*) " +
                $"FROM {(string)parameterConfiguration["parameters"]["butterflyValve"]["property_19"]["database"]} " +
                $"WHERE {(string)parameterConfiguration["parameters"]["butterflyValve"]["property_13"]["database"]} = \"{this.Fabrikant}\" " +
                $"AND {(string)parameterConfiguration["parameters"]["butterflyValve"]["property_14"]["database"]} = \"{this.Type}\" " +
                $"AND {(string)parameterConfiguration["parameters"]["butterflyValve"]["property_3"]["database"]} = {this.Dn}";
        }

        public override bool? GetParams()
        {
            List<object> parametersList;

            if (FileOperationsPipeAccessories.LookupButterflyValve(Query, QueryCount, ConnectionString, out parametersList))
            {
                if (FileOperations.IsFound(CallingParams, RememberMeFilePath, out List<string> parameters))
                {
                    List<object> correctList = new List<object>();

                    correctList.AddRange(parameters.GetRange(0, 12).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameters.GetRange(12, 6));

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
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Buitendiameter_totaal").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Lengte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Buitendiameter").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Staaf_lengte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Hendel_lengte").AsDouble(), 4));
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("Motor_lengte").AsDouble());
            this.RevitParameters.Add(this.PipeAccessory.LookupParameter("Motor_hoogte").AsDouble());
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Motor_breedte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Blade_dikte").AsDouble(), 4));
            this.RevitParameters.Add(Math.Round(this.PipeAccessory.LookupParameter("Blade_diameter").AsDouble(), 4));
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
            this.PipeAccessory.LookupParameter("Buitendiameter_totaal").Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter("Lengte").Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter("Buitendiameter").Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter("Staaf_lengte").Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter("Hendel_lengte").Set(0);
            this.PipeAccessory.LookupParameter("Motor_lengte").Set(0);
            this.PipeAccessory.LookupParameter("Motor_hoogte").Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter("Motor_breedte").Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter("Blade_dikte").Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter("Blade_diameter").Set(10 / feetToMm);
            this.PipeAccessory.LookupParameter("Wormwiel_diameter").Set(15 / feetToMm);
            this.PipeAccessory.LookupParameter("Wormwiel_lengte").Set(20 / feetToMm);
            this.PipeAccessory.LookupParameter("COD_Fabrikant").Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter("COD_Type").Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter("COD_Materiaal").Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter("COD_Productcode").Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter("COD_Omschrijving").Set("BESTAAT NIET!");
            this.PipeAccessory.LookupParameter("COD_Beschikbaar").Set("BESTAAT NIET!");
        }
    }
}
