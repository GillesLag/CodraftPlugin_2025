using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class HoldClouds : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                Document doc = commandData.Application.ActiveUIDocument.Document;

                IEnumerable<RevisionCloud> clouds = new FilteredElementCollector(doc)
                    .OfClass(typeof(RevisionCloud))
                    .WhereElementIsNotElementType()
                    .Cast<RevisionCloud>()
                    .Where(cloud => cloud.Name == "-");

                Color magenta = new Color(255, 0, 255);
                OverrideGraphicSettings ogs = new OverrideGraphicSettings();
                ogs.SetProjectionLineColor(magenta);

                Transaction t = new Transaction(doc, "HOLD CLOUDS");

                int wolkenAangepast = 0;

                t.Start();
                foreach (RevisionCloud cloud in clouds)
                {
                    Parameter param = cloud.get_Parameter(BuiltInParameter.ALL_MODEL_INSTANCE_COMMENTS);

                    if (!string.IsNullOrWhiteSpace(param.AsString()))
                    {
                        continue;
                    }

                    View view = (View)doc.GetElement(cloud.OwnerViewId);
                    view.SetElementOverrides(cloud.Id, ogs);

                    param.Set("ON HOLD");
                    wolkenAangepast++;
                }
                t.Commit();

                TaskDialog.Show("Hold Wolken", wolkenAangepast + " wolk(en) aangepast.");

                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }
    }
}
