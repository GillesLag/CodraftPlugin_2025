using Autodesk.Revit.DB;

namespace CodraftPlugin_UIDatabaseWPF.Model
{
    public class Tee : FittingModel
    {
        #region properties
        public double Nominale_Diameter_1 { get; set; }
        public double Nominale_Diameter_2 { get; set; }
        public double Nominale_Diameter_3 { get; set; }
        public double Buitendiameter_1 { get; set; }
        public double Buitendiameter_2 { get; set; }
        public double Buitendiameter_3 { get; set; }
        public double Hoek { get; set; }
        public double Uiteinde_1_Type { get; set; }
        public double Uiteinde_2_Type { get; set; }
        public double Uiteinde_3_Type { get; set; }
        public double Uiteinde_1_Lengte { get; set; }
        public double Uiteinde_2_Lengte { get; set; }
        public double Uiteinde_3_Lengte { get; set; }
        public double Uiteinde_1_Maat { get; set; }
        public double Uiteinde_2_Maat { get; set; }
        public double Uiteinde_3_Maat { get; set; }
        public double Lengte { get; set; }
        public double Center_Uiteinde_1 { get; set; }
        public double Center_Uiteinde_3 { get; set; }
        public double FlensDikte1 { get; set; }
        public double FlensDikte2 { get; set; }
        public double FlensDikte3 { get; set; }

        #endregion

        /// <summary>
        ///  A Tee with all the usefull parameters needed to make the teefitting in the document
        /// </summary>
        /// <param name="fabrikant"></param>
        /// <param name="type"></param>
        /// <param name="materiaal"></param>
        /// <param name="productCode"></param>
        /// <param name="beschikbaar"></param>
        /// <param name="omschrijving"></param>
        /// <param name="id"></param>
        /// <param name="fitting"></param>
        /// <param name="nominaleDiameter1"></param>
        /// <param name="nominaleDiameter2"></param>
        /// <param name="nominaleDiameter3"></param>
        /// <param name="buitendiameter1"></param>
        /// <param name="buitendiameter2"></param>
        /// <param name="buitendiameter3"></param>
        /// <param name="hoek"></param>
        /// <param name="uiteindeType1"></param>
        /// <param name="uiteindeType2"></param>
        /// <param name="uiteindeType3"></param>
        /// <param name="uiteindeLengte1"></param>
        /// <param name="uiteindeLengte2"></param>
        /// <param name="uiteindeLengte3"></param>
        /// <param name="uiteindeMaat1"></param>
        /// <param name="uiteindeMaat2"></param>
        /// <param name="uiteindeMaat3"></param>
        /// <param name="lengte"></param>
        /// <param name="centerUiteinde1"></param>
        /// <param name="centerUiteinde3"></param>
        /// <param name="flensDikte1"></param>
        /// <param name="flensDikte2"></param>
        /// <param name="flensDikte3"></param>
        public Tee(string fabrikant, string type, string materiaal, string productCode, string beschikbaar, string omschrijving, FamilyInstance fitting,
            double nominaleDiameter1, double nominaleDiameter2, double nominaleDiameter3, double buitendiameter1, double buitendiameter2, double buitendiameter3,
            double hoek, double uiteindeType1, double uiteindeType2, double uiteindeType3, double uiteindeLengte1, double uiteindeLengte2, double uiteindeLengte3,
            double uiteindeMaat1, double uiteindeMaat2, double uiteindeMaat3, double lengte, double centerUiteinde1, double centerUiteinde3, double flensDikte1,
            double flensDikte2, double flensDikte3)
            : base(fabrikant, type, materiaal, productCode, omschrijving, beschikbaar, fitting)
        {
            this.Nominale_Diameter_1 = nominaleDiameter1;
            this.Nominale_Diameter_2 = nominaleDiameter2;
            this.Nominale_Diameter_3 = nominaleDiameter3;
            this.Buitendiameter_3 = buitendiameter1;
            this.Buitendiameter_2 = buitendiameter2;
            this.Buitendiameter_3 = buitendiameter3;
            this.Hoek = hoek;
            this.Uiteinde_1_Type = uiteindeType1;
            this.Uiteinde_2_Type = uiteindeType2;
            this.Uiteinde_3_Type = uiteindeType3;
            this.Uiteinde_1_Lengte = uiteindeLengte1;
            this.Uiteinde_2_Lengte = uiteindeLengte2;
            this.Uiteinde_3_Lengte = uiteindeLengte3;
            this.Uiteinde_1_Maat = uiteindeMaat1;
            this.Uiteinde_2_Maat = uiteindeMaat2;
            this.Uiteinde_3_Maat = uiteindeMaat3;
            this.Lengte = lengte;
            this.Center_Uiteinde_1 = centerUiteinde1;
            this.Center_Uiteinde_3 = centerUiteinde3;
            this.FlensDikte1 = flensDikte1;
            this.FlensDikte2 = flensDikte2;
            this.FlensDikte3 = flensDikte3;
        }
    }
}
