using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodraftPlugin_UIDatabaseWPF.Model
{
    public class Tap : FittingModel
    {
        public double Nominale_Diameter { get; set; }
        public double Buitendiameter { get; set; }
        public double Lengte { get; set; }
        public double Lengte_Waarde { get; set; }

        public Tap(string fabrikant, string type, string materiaal, string productCode, string omschrijving, string beschikbaar, FamilyInstance fitting,
            double nominale_Diameter, double buitendiameter, double lengte, double maxDiameter)
            : base(fabrikant, type, materiaal, productCode, omschrijving, beschikbaar, fitting)
        {
            Nominale_Diameter = nominale_Diameter;
            Buitendiameter = buitendiameter;
            Lengte = lengte;
            Lengte_Waarde = maxDiameter / 2;
        }
    }
}
