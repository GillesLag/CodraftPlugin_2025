using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace CodraftPlugin_ChangeTagColor
{
    [Transaction(TransactionMode.Manual)]
    public class CurveChangeTagColor : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uiDoc = uiapp.ActiveUIDocument;
                Document doc = uiDoc.Document;
                View view = doc.ActiveView;

                GroupPickFilter gpf = new GroupPickFilter();
                Reference sel = uiDoc.Selection.PickObject(ObjectType.Element, gpf, "Select Duct/Pipe/Fitting or Accessory tag");
                Element elem = doc.GetElement(sel.ElementId);
                BuiltInCategory cat = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), elem.Category.Id.ToString());

                FilteredElementCollector tagList = new FilteredElementCollector(doc, view.Id)
                    .OfCategory(cat)
                    .WhereElementIsNotElementType();

                List<Element> eList = new List<Element>();

                foreach (IndependentTag e in tagList)
                {
                    Element i = e.GetTaggedLocalElement();
                    eList.Add(i);
                }

                List<Color> color = new List<Color>();

                BuiltInParameter builtInParam = (BuiltInParameter)Enum.Parse(typeof(BuiltInParameter), eList[0].GetParameters("System Type")[0].Id.ToString());

                foreach (Element e in eList)
                {
                    MEPSystemType system = doc.GetElement(e.get_Parameter(builtInParam).AsElementId()) as MEPSystemType;

                    byte red = system.LineColor.Red;
                    byte green = system.LineColor.Green;
                    byte blue = system.LineColor.Blue;

                    Color elemColor = new Color(red, green, blue);
                    color.Add(elemColor);
                }

                OverrideGraphicSettings overRide = new OverrideGraphicSettings();

                Transaction trans = new Transaction(doc, "change tag color");
                trans.Start();

                int index = 0;
                foreach (IndependentTag e in tagList)
                {
                    view.SetElementOverrides(e.Id, overRide.SetProjectionLineColor(color[index]));
                    index++;
                }

                trans.Commit();

                return Result.Succeeded;
            }

            catch (OperationCanceledException)
            {
                return Result.Cancelled;
            }

            catch (Exception)
            {
                return Result.Failed;
            }

        }
        public class GroupPickFilter : ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                int i = e.Category.Id.IntegerValue;
                if (i == ((int)BuiltInCategory.OST_PipeTags) || i == ((int)BuiltInCategory.OST_DuctTags)
                    || i == ((int)BuiltInCategory.OST_DuctAccessoryTags) || i == ((int)BuiltInCategory.OST_DuctFittingTags)
                    || i == ((int)BuiltInCategory.OST_PipeAccessoryTags) || i == ((int)BuiltInCategory.OST_PipeFittingTags))
                {
                    return true;
                }
                return false;
            }
            public bool AllowReference(Reference r, XYZ p)
            {
                return false;
            }
        }
    }
}
