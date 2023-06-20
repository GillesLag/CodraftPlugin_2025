using Autodesk.Revit.DB;

namespace CodraftPlugin_UIDatabaseWPF.Model
{
    public class Transition : FittingModel
    {
        #region properties
        public double Nominale_Diameter_1 { get; set; }
        public double Nominale_Diameter_2 { get; set; }
        public double Buitendiameter_1 { get; set; }
        public double Buitendiameter_2 { get; set; }
        public double Uiteinde_1_Type { get; set; }
        public double Uiteinde_2_Type { get; set; }
        public double Uiteinde_1_Lengte { get; set; }
        public double Uiteinde_2_Lengte { get; set; }
        public double Uiteinde_1_Maat { get; set; }
        public double Uiteinde_2_Maat { get; set; }
        public double FlensDikte1 { get; set; }
        public double FlensDikte2 { get; set; }
        public double Excentrisch { get; set; }
        public double Lengte { get; set; }

        #endregion

        /// <summary>
        ///  A transition with all the usefull parameters needed to make the transitionfitting in the document
        /// </summary>
        /// <param name="fabrikant"></param>
        /// <param name="type"></param>
        /// <param name="materiaal"></param>
        /// <param name="productCode"></param>
        /// <param name="beschikbaar"></param>
        /// <param name="omschrijving"></param>
        /// <param name="id"></param>
        /// <param name="fitting"></param>
        /// <param name="nominale_Diameter_1"></param>
        /// <param name="nominale_Diameter_2"></param>
        /// <param name="buitendiameter_1"></param>
        /// <param name="buitendiameter_2"></param>
        /// <param name="uiteinde_1_Type"></param>
        /// <param name="uiteinde_2_Type"></param>
        /// <param name="uiteinde_1_Lengte"></param>
        /// <param name="uiteinde_2_Lengte"></param>
        /// <param name="uiteinde_1_Maat"></param>
        /// <param name="uiteinde_2_Maat"></param>
        /// <param name="flensDikte1"></param>
        /// <param name="flensDikte2"></param>
        /// <param name="lengte"></param>
        public Transition(string fabrikant, string type, string materiaal, string productCode, string beschikbaar, string omschrijving, FamilyInstance fitting,
            double nominale_Diameter_1, double nominale_Diameter_2, double buitendiameter_1, double buitendiameter_2, double uiteinde_1_Type, double uiteinde_2_Type,
            double uiteinde_1_Lengte, double uiteinde_2_Lengte, double uiteinde_1_Maat, double uiteinde_2_Maat, double flensDikte1, double flensDikte2, double lengte)
            : base(fabrikant, type, materiaal, productCode, omschrijving, beschikbaar, fitting)
        {
            this.Nominale_Diameter_1 = nominale_Diameter_1;
            this.Nominale_Diameter_2 = nominale_Diameter_2;
            this.Buitendiameter_1 = buitendiameter_1;
            this.Buitendiameter_2 = buitendiameter_2;
            this.Uiteinde_1_Type = uiteinde_1_Type;
            this.Uiteinde_2_Type = uiteinde_2_Type;
            this.Uiteinde_1_Lengte = uiteinde_1_Lengte;
            this.Uiteinde_2_Lengte = uiteinde_2_Lengte;
            this.Uiteinde_1_Maat = uiteinde_1_Maat;
            this.Uiteinde_2_Maat = uiteinde_2_Maat;
            this.FlensDikte1 = flensDikte1;
            this.FlensDikte2 = flensDikte2;
            this.Lengte = lengte;
            this.Excentrisch = fitting.LookupParameter("COD_Excentrisch").AsDouble();
        }
    }
}
