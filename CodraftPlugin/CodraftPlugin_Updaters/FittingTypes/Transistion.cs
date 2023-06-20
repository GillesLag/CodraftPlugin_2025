using Autodesk.Revit.DB;
using CodraftPlugin_DAL;
using CodraftPlugin_Exceptions;
using CodraftPlugin_UIDatabaseWPF;
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

        public Transition(FamilyInstance transistion, Document doc, string databaseMapPath, string textFilesMapPath)
            : base(transistion, doc, databaseMapPath, textFilesMapPath)
        {
            double nd1 = Math.Round(transistion.LookupParameter("COD_c1_Nominale_diameter").AsDouble()*feetToMm);
            double nd2 = Math.Round(transistion.LookupParameter("COD_c2_Nominale_diameter").AsDouble()*feetToMm);

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

            this.Excentrisch = transistion.LookupParameter("COD_Excentrisch").AsInteger();

            StrSQL = $"SELECT *" +
                $" FROM BMP_TransitionTbl" +
                $" WHERE Nominale_diameter_1 = {this.Nd1}" +
                $" AND Nominale_diameter_2 = {this.Nd2}" +
                $" AND Excentrisch = \"{this.Excentrisch}\";";

            StrCountSQL = $"SELECT COUNT(*)" +
                $" FROM BMP_TransitionTbl" +
                $" WHERE Nominale_diameter_1 = {this.Nd1}" +
                $" AND Nominale_diameter_2 = {this.Nd2}" +
                $" AND Excentrisch = \"{this.Excentrisch}\";";

        }

        public List<object> GetParamsFromDB()
        {
            // Check for multiple rows in database.
            if (FileOperations.LookupTransistion(StrSQL, StrCountSQL, ConnectionString, out List<object> paramList))
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

                Window w = new UIDatabase(ConnectionString, StrSQL, Doc, Fi, TextFilesMapPath, DatabaseFilePath, RememberMeFile, parameters, switchNds, Excentrisch);
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
                fittingParams.Add(Math.Round(Fi.LookupParameter("COD_c1_Buitendiameter").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("COD_c2_Buitendiameter").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Lengte").AsDouble(), 4));
                fittingParams.Add(Convert.ToDouble(Fi.LookupParameter("Uiteinde_1_type").AsInteger()));
                fittingParams.Add(Convert.ToDouble(Fi.LookupParameter("Uiteinde_2_type").AsInteger()));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_1_maat").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_2_maat").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_1_lengte").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_2_lengte").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Flens_dikte_1").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Flens_dikte_2").AsDouble(), 4));
                fittingParams.Add(Fi.LookupParameter("COD_Fabrikant").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Type").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Materiaal").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Productcode").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Omschrijving").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Beschikbaar").AsString());
            }
            else
            {
                fittingParams.Add(Math.Round(Fi.LookupParameter("COD_c2_Buitendiameter").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("COD_c1_Buitendiameter").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Lengte").AsDouble(), 4));
                fittingParams.Add(Convert.ToDouble(Fi.LookupParameter("Uiteinde_2_type").AsInteger()));
                fittingParams.Add(Convert.ToDouble(Fi.LookupParameter("Uiteinde_1_type").AsInteger()));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_2_maat").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_1_maat").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_2_lengte").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_1_lengte").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Flens_dikte_2").AsDouble(), 4));
                fittingParams.Add(Math.Round(Fi.LookupParameter("Flens_dikte_1").AsDouble(), 4));
                fittingParams.Add(Fi.LookupParameter("COD_Fabrikant").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Type").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Materiaal").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Productcode").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Omschrijving").AsString());
                fittingParams.Add(Fi.LookupParameter("COD_Beschikbaar").AsString());
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
                    Fi.LookupParameter("COD_Fabrikant").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Type").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Materiaal").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Productcode").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Omschrijving").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Beschikbaar").AsString() == "nee" &&

                    Fi.LookupParameter("COD_c1_Buitendiameter").AsValueString() == "15" &&
                    Fi.LookupParameter("COD_c2_Buitendiameter").AsValueString() == "15" &&
                    Fi.LookupParameter("Lengte").AsValueString() == "15" &&
                    Fi.LookupParameter("Uiteinde_1_type").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_2_type").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_1_maat").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_2_maat").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_1_lengte").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_2_lengte").AsDouble() == 0
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
