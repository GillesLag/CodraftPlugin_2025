using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodraftPlugin_PipeAccessoriesWPF.Model
{
    public class Strainer : BaseAccessory
    {
        public double BuitenDiameter { get; set; }
        public double Hoogte { get; set; }
        public double Lengte { get; set; }
        public double Offset { get; set; }
        public int UiteindeType1 { get; set; }
        public int UiteindeType2 { get; set; }
        public double L1 { get; set; }
        public double L2 { get; set; }
        public double UiteindeMaat1 { get; set; }
        public double UiteindeMaat2 { get; set; }

        public Strainer(FamilyInstance accessory, string fabrikant, string type, string materiaal, string productCode, string omschrijving,
            string beschikbaar, int nominaleDiameter, double buitenDiameter, double hoogte, double lengte, double offset, int uiteindeType1,
            int uiteindeType2, double l1, double l2, double uiteindeMaat1, double uiteindeMaat2)
            : base(accessory, fabrikant, type, materiaal, productCode, omschrijving, beschikbaar, nominaleDiameter)
        {
            BuitenDiameter = buitenDiameter;
            Hoogte = hoogte;
            Lengte = lengte;
            Offset = offset;
            UiteindeType1 = uiteindeType1;
            UiteindeType2 = uiteindeType2;
            L1 = l1;
            L2 = l2;
            UiteindeMaat1 = uiteindeMaat1;
            UiteindeMaat2 = uiteindeMaat2;
        }
    }
}
