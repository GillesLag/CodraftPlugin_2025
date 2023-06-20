using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodraftPlugin_PipeAccessoriesWPF.Model
{
    public class BalanceValve : BaseAccessory
    {
        public double Lengte { get; set; }
        public double BuitenDiameter { get; set; }
        public int UiteindeType1 { get; set; }
        public int UiteindeType2 { get; set; }
        public double UiteindeMaat1 { get; set; }
        public double UiteindeMaat2 { get; set; }
        public double L1 { get; set; }
        public double L2 { get; set; }


        public BalanceValve(FamilyInstance accessory, string fabrikant, string type, string materiaal, string productCode, string omschrijving,
            string beschikbaar, int nominaleDiameter, double lengte, double buitenDiameter, int uiteindeType1, int uiteindeType2, double uiteindeMaat1,
            double uiteindeMaat2, double l1, double l2)
            : base(accessory, fabrikant, type, materiaal, productCode, omschrijving, beschikbaar, nominaleDiameter)
        {
            Lengte = lengte;
            BuitenDiameter = buitenDiameter;
            UiteindeType1 = uiteindeType1;
            UiteindeType2 = uiteindeType2;
            UiteindeMaat1 = uiteindeMaat1;
            UiteindeMaat2 = uiteindeMaat2;
            L1 = l1;
            L2 = l2;
        }
    }
}
