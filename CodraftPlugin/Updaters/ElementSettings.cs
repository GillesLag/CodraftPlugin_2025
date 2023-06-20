using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;B
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB;

namespace CodraftPlugin_Settings
{
    public class ElementSettings
    {
        public static void SetParamtersPipe(Pipe pipe, ElementId systemType)
        {
            // Pipe parameters
            string startBoven = "COD_bovenkant_buis_start";
            string eindeBoven = "COD_bovenkant_buis_einde";
            string startCenter = "COD_center_buis_start";
            string eindeCenter = "COD_center_buis_einde";
            string startOnder = "COD_onderkant_buis_start";
            string eindeOnder = "COD_onderkant_buis_einde";
            double startOffset = pipe.get_Parameter(BuiltInParameter.RBS_START_OFFSET_PARAM).AsDouble();
            double eindeOffset = pipe.get_Parameter(BuiltInParameter.RBS_END_OFFSET_PARAM).AsDouble();
            double diameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER).AsDouble();

            try
            {
                // Set parameters
                pipe.SetSystemType(systemType);

                pipe.LookupParameter(startBoven).Set(startOffset > eindeOffset ? startOffset + diameter / 2 : eindeOffset + diameter / 2);
                pipe.LookupParameter(eindeBoven).Set(eindeOffset > startOffset ? startOffset + diameter / 2 : eindeOffset + diameter / 2);
                pipe.LookupParameter(startCenter).Set(startOffset > eindeOffset ? startOffset : eindeOffset);
                pipe.LookupParameter(eindeCenter).Set(eindeOffset > startOffset ? startOffset : eindeOffset);
                pipe.LookupParameter(startOnder).Set(startOffset > eindeOffset ? startOffset - diameter / 2 : eindeOffset - diameter / 2);
                pipe.LookupParameter(eindeOnder).Set(eindeOffset > startOffset ? startOffset - diameter / 2 : eindeOffset - diameter / 2);
            }
            catch (NullReferenceException)
            {
                TaskDialog td = new TaskDialog("ProjectParameters Fout");
                td.MainInstruction = "Niet alle project parameters zijn toegevoegd voor de pipes.";
                td.ExpandedContent = "De volgende project paramters moeten toegevoegd zijn in het project voor alle pipes.\n" +
                    "COD_bovenkant_buis_start,\n" +
                    "COD_bovenkant_buis_einde,\n" +
                    "COD_center_buis_start,\n" +
                    "COD_center_buis_einde,\n" +
                    "COD_onderkant_buis_start,\n" +
                    "COD_onderkant_buis_einde";
                td.Show();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Fout", ex.Message);
            }
        }
    }
}
