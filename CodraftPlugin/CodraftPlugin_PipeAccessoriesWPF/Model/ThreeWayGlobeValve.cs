using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodraftPlugin_PipeAccessoriesWPF.Model
{
    public class ThreeWayGlobeValve : BaseAccessory
    {
        public double BuitenDiameter { get; set; }
        public double Lengte { get; set; }
        public double Lengte3 { get; set; }
        public int UiteindeType1 { get; set; }
        public int UiteindeType2 { get; set; }
        public int UiteindeType3 { get; set; }
        public double L1 { get; set; }
        public double L2 { get; set; }
        public double L3 { get; set; }
        public double UiteindeMaat1 { get; set; }
        public double UiteindeMaat2 { get; set; }
        public double UiteindeMaat3 { get; set; }
        public double MotorLengte { get; set; }
        public double MotorBreedte { get; set; }
        public double MotorHoogte { get; set; }
        public double HoogteOperator { get; set; }
        public double WormwielDiameter { get; set; }
        public double WormwielLengte { get; set; }

        public ThreeWayGlobeValve(FamilyInstance accessory, string fabrikant, string type, string materiaal, string productCode, string omschrijving,
            string beschikbaar, int nominaleDiameter, double buitenDiameter, double lengte, double lengte3, int uiteindeType1, int uiteindeType2, int uiteindeType3,
            double l1, double l2, double l3, double uiteindeMaat1, double uiteindeMaat2, double uiteindeMaat3, double motorLengte, double motorBreedte,
            double motorHoogte, double hoogteOperator, double wormwielDiameter, double wormwielLengte)
            : base(accessory, fabrikant, type, materiaal, productCode, omschrijving, beschikbaar, nominaleDiameter)
        {
            BuitenDiameter = buitenDiameter;
            WormwielDiameter = wormwielDiameter;
            MotorLengte = motorLengte;
            MotorBreedte = motorBreedte;
            MotorHoogte = motorHoogte;
            HoogteOperator = hoogteOperator;
            Lengte = lengte;
            Lengte3 = lengte3;
            UiteindeType1 = uiteindeType1;
            UiteindeType2 = uiteindeType2;
            UiteindeType3 = uiteindeType3;
            L1 = l1;
            L2 = l2;
            L3 = l3;
            UiteindeMaat1 = uiteindeMaat1;
            UiteindeMaat2 = uiteindeMaat2;
            UiteindeMaat3 = uiteindeMaat3;
            WormwielLengte = wormwielLengte;
        }
    }
}
