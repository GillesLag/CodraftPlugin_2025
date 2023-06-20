using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodraftPlugin_PipeAccessoriesWPF.Model
{
    public class BaseAccessory
    {
        public string Fabrikant { get; set; }
        public string Type { get; set; }
        public string Materiaal { get; set; }
        public int NominaleDiameter { get; set; }
        public string ProductCode { get; set; }
        public string Omschrijving { get; set; }
        public string Beschikbaar { get; set; }
        
        public FamilyInstance Accessory { get; set; }

        public BaseAccessory(FamilyInstance accessory, string fabrikant, string type, string materiaal, string productCode, string omschrijving, string beschikbaar, int nominaleDiameter)
        {
            Fabrikant = fabrikant;
            Type = type;
            Materiaal = materiaal;
            ProductCode = productCode;
            Omschrijving = omschrijving;
            Beschikbaar = beschikbaar;
            Accessory = accessory;
            NominaleDiameter = nominaleDiameter;
        }
    }
}
