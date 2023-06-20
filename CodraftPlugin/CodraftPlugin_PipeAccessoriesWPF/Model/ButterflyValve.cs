using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace CodraftPlugin_PipeAccessoriesWPF.Model
{
    public class ButterflyValve : BaseAccessory
    {
        public double BuitenDiameterTotaal { get; set; }
        public double Lengte { get; set; }
        public double BuitenDiameter { get; set; }
        public double StaafLengte { get; set; }
        public double HendelLengte { get; set; }
        public double MotorLengte { get; set; }
        public double MotorHoogte { get; set; }
        public double MotorBreedte { get; set; }
        public double BladeDikte { get; set; }
        public double BladeDiameter { get; set; }
        public double WormwielDiameter { get; set; }
        public double WormwielLengte { get; set; }

        public ButterflyValve(FamilyInstance accessory, string fabrikant, string type, string materiaal, string productCode, string omschrijving,
            string beschikbaar, int nominaleDiameter, double buitenDiameterTotaal, double lengte, double buitenDiameter, double staafLengte,
            double hendelLengte, double motorLengte, double motorHoogte, double motorBreedte, double bladeDikte, double bladeDiameter,
            double wormwielDiameter, double wormwielLengte)
            : base(accessory, fabrikant, type, materiaal, productCode, omschrijving, beschikbaar, nominaleDiameter)
        {
            BuitenDiameterTotaal = buitenDiameterTotaal;
            Lengte = lengte;
            BuitenDiameter = buitenDiameter;
            StaafLengte = staafLengte;
            HendelLengte = hendelLengte;
            MotorLengte = motorLengte;
            MotorHoogte = motorHoogte;
            MotorBreedte = motorBreedte;
            BladeDikte = bladeDikte;
            BladeDiameter = bladeDiameter;
            WormwielDiameter = wormwielDiameter;
            WormwielLengte = wormwielLengte;
        }
    }
}
