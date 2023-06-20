using Autodesk.Revit.DB;

namespace CodraftPlugin_UIDatabaseWPF.Model
{
    public class FittingModel
    {
        #region Properties
        public string Fabrikant { get; set; }
        public string Type { get; set; }
        public string Materiaal { get; set; }
        public string ProductCode { get; set; }
        public string Omschrijving { get; set; }
        public string Beschikbaar { get; set; }
        public int Id { get; set; }
        public FamilyInstance Fitting { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// the base fitting with all the usefull information for the fitting in the document
        /// </summary>
        /// <param name="fabrikant"></param>
        /// <param name="type"></param>
        /// <param name="materiaal"></param>
        /// <param name="productCode"></param>
        /// <param name="omschrijving"></param>
        /// <param name="beschikbaar"></param>
        /// <param name="fitting"></param>
        public FittingModel(string fabrikant, string type, string materiaal, string productCode, string omschrijving, string beschikbaar, FamilyInstance fitting)
        {
            this.Fabrikant = fabrikant;
            this.Type = type;
            this.Materiaal = materiaal;
            this.ProductCode = productCode;
            this.Omschrijving = omschrijving;
            this.Beschikbaar = beschikbaar;
            this.Fitting = fitting;
            this.Id = fitting.Id.IntegerValue;
        }
        #endregion
    }
}
