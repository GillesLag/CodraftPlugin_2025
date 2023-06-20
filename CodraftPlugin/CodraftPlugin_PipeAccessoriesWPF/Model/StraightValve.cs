using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodraftPlugin_PipeAccessoriesWPF.Model
{
    public class StraightValve : BaseAccessory
    {
        public double Lengte { get; set; }
        public double HendelLengte { get; set; }
        public double HendelBreedte { get; set; }
        public double HendelHoogte { get; set; }
        public double MotorLengte { get; set; }
        public double MotorBreedte { get; set; }
        public double MotorHoogte { get; set; }
        public double WormwielStraal { get; set; }
        public double WormwielStaafStraal { get; set; }
        public double OperatorHoogte { get; set; }
        public double VlinderhendelDiameter { get; set; }
        public int UiteindeType1 { get; set; }
        public int UiteindeType2 { get; set; }
        public double UiteindeMaat1 { get; set; }
        public double UiteindeMaat2 { get; set; }
        public double L1 { get; set; }
        public double L2 { get; set; }
        public double BuitenDiameter { get; set; }

        public StraightValve(FamilyInstance accessory, string fabrikant, string type, string materiaal, string productCode, string omschrijving, 
            string beschikbaar, int nominaleDiameter, double lengte, double hendelLengte, double hendelBreedte, double hendelHoogte,
            double motorLengte, double motorBreedte, double motorHoogte, double wormwielStraal, double wormwielStaafStraal, double operatorHoogte,
            double vlinderhendelDiameter, int uiteindeType1, int uiteindeType2, double uiteindeMaat1, double uiteindeMaat2, double l1, double l2, double buitenDiameter) 
            : base(accessory, fabrikant, type, materiaal, productCode, omschrijving, beschikbaar, nominaleDiameter)
        {
            NominaleDiameter = nominaleDiameter;
            Lengte = lengte;
            HendelLengte = hendelLengte;
            HendelBreedte = hendelBreedte;
            HendelHoogte = hendelHoogte;
            MotorLengte = motorLengte;
            MotorBreedte = motorBreedte;
            MotorHoogte = motorHoogte;
            WormwielStraal = wormwielStraal;
            WormwielStaafStraal = wormwielStaafStraal;
            OperatorHoogte = operatorHoogte;
            VlinderhendelDiameter = vlinderhendelDiameter;
            UiteindeType1 = uiteindeType1;
            UiteindeType2 = uiteindeType2;
            UiteindeMaat1 = uiteindeMaat1;
            UiteindeMaat2 = uiteindeMaat2;
            L1 = l1;
            L2 = l2;
            BuitenDiameter = buitenDiameter;
        }
    }
}
