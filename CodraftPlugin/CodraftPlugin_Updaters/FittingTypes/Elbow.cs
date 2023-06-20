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

        public Elbow(FamilyInstance elbow, Document doc, string databaseMapPath, string textFilesMapPath)
            : base(elbow, doc, databaseMapPath, textFilesMapPath)
        {
            this.Nd1 = Math.Round(elbow.LookupParameter("COD_c1_Nominale_diameter").AsDouble()*feetToMm).ToString();
            this.Nd2 = Math.Round(elbow.LookupParameter("COD_c2_Nominale_diameter").AsDouble()*feetToMm).ToString();
            this.HoekTolerantie = Math.Round(elbow.LookupParameter("Hoek_tolerantie").AsDouble() * radiansToDegrees).ToString();
            this.Hoek = Math.Round(elbow.LookupParameter("COD_c1_Hoek").AsDouble() * radiansToDegrees).ToString();

            if (double.Parse(this.Hoek) > 45) this.HoekStandaard = "90";

            StrSQL = $"SELECT *" +
                $" FROM BMP_ElbowTbl" +
                $" WHERE Nominale_diameter_1 = {this.Nd1}" +
                $" AND Nominale_diameter_2 = {this.Nd2}" +
                $" AND Standaard_hoek = {this.HoekTolerantie};";

            StrCountSQL = $"SELECT COUNT(*)" +
                $" FROM BMP_ElbowTbl" +
                $" WHERE Nominale_diameter_1 = {this.Nd1}" +
                $" AND Nominale_diameter_2 = {this.Nd2}" +
                $" AND Standaard_hoek = {this.HoekTolerantie};";

            StrHoekInkortbaarSQL = $"SELECT Hoek_inkortbaar" +
                $" FROM BMP_ElbowTbl" +
                $" WHERE Nominale_diameter_1 = {this.Nd1}" +
                $" AND Nominale_diameter_2 = {this.Nd2}";
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
                StrSQL = $"SELECT *" +
                    $" FROM BMP_ElbowTbl" +
                    $" WHERE Nominale_diameter_1 = {this.Nd1}" +
                    $" AND Nominale_diameter_2 = {this.Nd2}" +
                    $" AND Standaard_hoek = {this.HoekStandaard};";

                StrCountSQL = $"SELECT COUNT(*)" +
                    $" FROM BMP_ElbowTbl" +
                    $" WHERE Nominale_diameter_1 = {this.Nd1}" +
                    $" AND Nominale_diameter_2 = {this.Nd2}" +
                    $" AND Standaard_hoek = {this.HoekStandaard};";
            }

            // Check for multiple rows in database.
            if (FileOperations.LookupElbow(StrSQL, StrCountSQL, ConnectionString, out List<object> paramList))
            {
                List<string> parameters = new List<string>() { Nd1, Nd2, HoekTolerantie, Fi.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString() };

                if (FileOperations.IsFound(parameters, RememberMeFile, out List<string> parameterList))
                {
                    List<object> correctList = new List<object>();

                    correctList.AddRange(parameterList.GetRange(0, 11).Select(x => (object)double.Parse(x)));
                    correctList.AddRange(parameterList.GetRange(11, 6));

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
            fittingParams.Add(Math.Round(Fi.LookupParameter("Center_straal").AsDouble(), 4));
            fittingParams.Add(Convert.ToDouble(Fi.LookupParameter("Uiteinde_1_type").AsInteger()));
            fittingParams.Add(Convert.ToDouble(Fi.LookupParameter("Uiteinde_2_type").AsInteger()));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_1_maat").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_2_maat").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_1_lengte").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Uiteinde_2_lengte").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Flens_dikte").AsDouble(), 4));
            fittingParams.Add(Math.Round(Fi.LookupParameter("Standaard_hoek").AsDouble(), 4));
            fittingParams.Add(Fi.LookupParameter("COD_Fabrikant").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Type").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Materiaal").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Productcode").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Omschrijving").AsString());
            fittingParams.Add(Fi.LookupParameter("COD_Beschikbaar").AsString());

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
                    Fi.LookupParameter("COD_Fabrikant").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Type").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Materiaal").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Productcode").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Omschrijving").AsString() == "BESTAAT NIET!" &&
                    Fi.LookupParameter("COD_Beschikbaar").AsString() == "nee" &&

                    Fi.LookupParameter("COD_c1_Buitendiameter").AsValueString() == "15" &&
                    Fi.LookupParameter("COD_c2_Buitendiameter").AsValueString() == "15" &&
                    Fi.LookupParameter("Center_straal").AsValueString() == "30" &&
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
