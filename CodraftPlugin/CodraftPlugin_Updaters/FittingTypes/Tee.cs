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

        public Tee(FamilyInstance tee, Document doc, string databaseMapPath, string textFilesMapPath)
            : base(tee, doc, databaseMapPath, textFilesMapPath)
        {
            this.Nd1 = Math.Round(tee.LookupParameter("COD_c1_Nominale_diameter").AsDouble()*feetToMm).ToString();
            this.Nd2 = Math.Round(tee.LookupParameter("COD_c2_Nominale_diameter").AsDouble() * feetToMm).ToString();
            this.Nd3 = Math.Round(tee.LookupParameter("COD_c3_Nominale_diameter").AsDouble() * feetToMm).ToString();
            this.Hoek = Math.Round(tee.LookupParameter("Standaard_hoek").AsDouble() * radiansToDegrees).ToString();

            StrSQL = $"SELECT *" +
                $" FROM BMP_TeeTbl" +
                $" WHERE Nominale_diameter_1 = {this.Nd1}" +
                $" AND Nominale_diameter_2 = {this.Nd2}" +
                $" AND Nominale_diameter_3 = {this.Nd3}" +
                $" AND Standaard_hoek = {this.Hoek};";

            StrCountSQL = $"SELECT COUNT(*)" +
                $" FROM BMP_TeeTbl" +
                $" WHERE Nominale_diameter_1 = {this.Nd1}" +
                $" AND Nominale_diameter_2 = {this.Nd2}" +
                $" AND Nominale_diameter_3 = {this.Nd3}" +
                $" AND Standaard_hoek = {this.Hoek};";
        }

        /// <summary>
        /// Get all params from the database. 
        /// </summary>
        /// <returns>Returns a list with parameters.</returns>
        /// <exception cref="FittingDoesNotExistException">Throws an exception if the the fitting does not exist in the database.</exception>
        public List<object> GetParamsFromDB()
        {
            // Check for multiple rows in database.
            if (FileOperations.LookupTee(StrSQL, StrCountSQL, ConnectionString, out List<object> paramList))
            {
                List<string> parameters = new List<string>() { Nd1, Nd2, Nd3, Hoek, Fi.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString() };

                if (FileOperations.IsFound(parameters, RememberMeFile, out List<string> parameterList))
                {
                    List<object> correctList = new List<object>();

                    correctList.AddRange(parameterList.GetRange(0, 19).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameterList.GetRange(19, 6));

                    return correctList;
                }

                Window w = new UIDatabase(ConnectionString, StrSQL, Doc, Fi, TextFilesMapPath, DatabaseFilePath, RememberMeFile, parameters);
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
            List<object> fittingParams = new List<object>();

            fittingParams.Add(Math.Round(Fi.LookupParameter("COD_c1_Buitendiameter").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("COD_c2_Buitendiameter").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("COD_c3_Buitendiameter").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Lengte_waarde").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Center_uiteinde_3_waarde").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Center_uiteinde_1_waarde").AsDouble(), 4));
            fittingParams.Add(Convert.ToDouble(Fi.LookupParameter("Uiteinde_1_type").AsInteger()));
            fittingParams.Add(Convert.ToDouble(Fi.LookupParameter("Uiteinde_2_type").AsInteger()));
            fittingParams.Add(Convert.ToDouble(Fi.LookupParameter("Uiteinde_3_type").AsInteger()));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_1_maat").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_2_maat").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_3_maat").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_1_lengte").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_2_lengte").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_3_lengte").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Flens_dikte_1").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Flens_dikte_2").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Flens_dikte_3").AsDouble(), 4));
            fittingParams.Add(Fi.LookupParameter("COD_Fabrikant").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Type").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Materiaal").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Productcode").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Omschrijving").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Beschikbaar").AsString());

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
                    Fi.LookupParameter("COD_Fabrikant").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Type").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Materiaal").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Productcode").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Omschrijving").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Beschikbaar").AsString() == "nee" &&

                    Fi.LookupParameter("COD_c1_Buitendiameter").AsValueString() == "15" &&
                    Fi.LookupParameter("COD_c2_Buitendiameter").AsValueString() == "15" &&
                    Fi.LookupParameter("COD_c3_Buitendiameter").AsValueString() == "15" &&
                    Fi.LookupParameter("Center_uiteinde_3_waarde").AsValueString() == "15" &&
                    Fi.LookupParameter("Center_uiteinde_1_waarde").AsValueString() == "15" &&
                    Fi.LookupParameter("Lengte_waarde").AsValueString() == "15" &&
                    Fi.LookupParameter("Uiteinde_1_type").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_2_type").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_3_type").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_1_maat").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_2_maat").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_3_maat").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_1_lengte").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_2_lengte").AsDouble() == 0 &&
                    Fi.LookupParameter("Uiteinde_3_lengte").AsDouble() == 0
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
