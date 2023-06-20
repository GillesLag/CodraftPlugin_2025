using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using CodraftPlugin_DAL;
using CodraftPlugin_Exceptions;
using CodraftPlugin_UIDatabaseWPF;
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

        public Tap(FamilyInstance tap, Document doc, string databaseMapPath, string textFilesMapPath)
            : base(tap, doc, databaseMapPath, textFilesMapPath)
        {
            this.Nd1 = Math.Round(tap.LookupParameter("COD_c1_Nominale_diameter").AsDouble()*feetToMm).ToString();

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
                $" FROM BMP_StubTbl" +
                $" WHERE BMP_StubTbl.Nominale_diameter={this.Nd1};";

            this.StrCountSQL = $"SELECT COUNT(*)" +
                $" FROM BMP_StubTbl" +
                $" WHERE BMP_StubTbl.Nominale_diameter={this.Nd1};";
        }

        public List<object> GetParamsFromDB()
        {
            // Check for multiple rows in database.
            if (FileOperations.LookupTap(StrSQL, StrCountSQL, ConnectionString, MaxDiameter, out List<object> paramList))
            {
                List<string> parameters = new List<string>() { Nd1, Fi.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString() };

                if (FileOperations.IsFound(parameters, RememberMeFile, out List<string> parameterList))
                {
                    List<object> correctList = new List<object>();

                    correctList.AddRange(parameterList.GetRange(0, 3).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameterList.GetRange(3, 6));

                    return correctList;
                }

                Window w = new UIDatabase(ConnectionString, StrSQL, Doc, Fi, TextFilesMapPath, DatabaseFilePath, RememberMeFile, parameters, false, 0 , MaxDiameter);
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
            List<object> fittingParams = new List<object>();

            fittingParams.Add(Math.Round(Fi.LookupParameter("COD_c1_Buitendiameter").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Lengte").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Lengte_waarde").AsDouble(), 4));
            fittingParams.Add(Fi.LookupParameter("COD_Fabrikant").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Type").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Materiaal").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Productcode").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Omschrijving").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Beschikbaar").AsString());

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
                    Fi.LookupParameter("COD_Fabrikant").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Type").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Materiaal").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Productcode").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Omschrijving").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Beschikbaar").AsString() == "nee" &&

                    Fi.LookupParameter("COD_c1_Buitendiameter").AsValueString() == "15" &&
                    Fi.LookupParameter("Lengte").AsValueString() == "15" &&
                    Fi.LookupParameter("Lengte_waarde").AsValueString() == "15"
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
