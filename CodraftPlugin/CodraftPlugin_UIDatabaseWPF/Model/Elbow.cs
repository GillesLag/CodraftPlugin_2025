using Autodesk.Revit.DB;

namespace CodraftPlugin_UIDatabaseWPF.Model
{
    public class Elbow : FittingModel
    {
        #region properties
        public double Nominale_Diameter_1 { get; set; }
        public double Nominale_Diameter_2 { get; set; }
        public double Buitendiameter_1 { get; set; }
        public double Buitendiameter_2 { get; set; }
        public double Hoek { get; set; }
        public double Inkortbaar { get; set; }
        public double Uiteinde_1_Type { get; set; }
        public double Uiteinde_2_Type { get; set; }
        public double Uiteinde_1_Lengte { get; set; }
        public double Uiteinde_2_Lengte { get; set; }
        public double Uiteinde_1_Maat { get; set; }
        public double Uiteinde_2_Maat { get; set; }
        public double FlensDikte { get; set; }
        public double CenterStraal { get; set; }

        #endregion

        /// <summary>
        /// An elbow with all the usefull parameters needed to make the elbowfitting in the document
        /// </summary>
        /// <param name="fabrikant"></param>
        /// <param name="type"></param>
        /// <param name="materiaal"></param>
        /// <param name="productCode"></param>
        /// <param name="beschikbaar"></param>
        /// <param name="omschrijving"></param>
        /// <param name="id"></param>
        /// <param name="fitting"></param>
        /// <param name="nd1"></param>
        /// <param name="nd2"></param>
        /// <param name="Ø1"></param>
        /// <param name="Ø2"></param>
        /// <param name="hoek"></param>
        /// <param name="inkortbaar"></param>
        /// <param name="uiteindeType1"></param>
        /// <param name="uiteindeType2"></param>
        /// <param name="uiteindeLengte1"></param>
        /// <param name="uiteindeLengte2"></param>
        /// <param name="uiteindeMaat1"></param>
        /// <param name="uiteindeMaat2"></param>
        /// <param name="flensDikte"></param>
        /// <param name="centerStraal"></param>
        public Elbow(string fabrikant, string type, string materiaal, string productCode, string beschikbaar, string omschrijving, FamilyInstance fitting,
            double nd1, double nd2, double Ø1, double Ø2, double hoek, double inkortbaar, double uiteindeType1, double uiteindeType2, double uiteindeLengte1,
            double uiteindeLengte2, double uiteindeMaat1, double uiteindeMaat2, double flensDikte, double centerStraal)
            : base(fabrikant, type, materiaal, productCode, beschikbaar, omschrijving, fitting)
        {
            this.Nominale_Diameter_1 = nd1;
            this.Nominale_Diameter_2 = nd2;
            this.Buitendiameter_1 = Ø1;
            this.Buitendiameter_2 = Ø2;
            this.Hoek = hoek;
            this.Inkortbaar = inkortbaar;
            this.Uiteinde_1_Type = uiteindeType1;
            this.Uiteinde_2_Type = uiteindeType2;
            this.Uiteinde_1_Lengte = uiteindeLengte1;
            this.Uiteinde_2_Lengte = uiteindeLengte2;
            this.Uiteinde_1_Maat = uiteindeMaat1;
            this.Uiteinde_2_Maat = uiteindeMaat2;
            this.FlensDikte = flensDikte;
            this.CenterStraal = centerStraal;
        }
    }
}
