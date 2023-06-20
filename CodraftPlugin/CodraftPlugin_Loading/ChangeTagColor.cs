using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Plumbing;
using System.Windows.Markup.Localizer;
using Autodesk.Revit.UI.Events;
using System.Windows.Input;
using Autodesk.Revit.DB.Mechanical;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class ChangeTagColor : IExternalCommand
    {
        private bool _isRunning = true;
        private UIApplication _app;
        private UIDocument _uidoc;
        private Document _doc;
        private bool _wholeDocument;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            _app = commandData.Application;
            _uidoc = _app.ActiveUIDocument;
            _doc = _uidoc.Document;

            TaskDialogResult answer =  TaskDialog.Show("ChangeTagColor", "Change the tag color only in this view?(Yes)\nOr in the whole document?(No)", TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No);

            switch (answer)
            {
                case TaskDialogResult.Yes:
                    _wholeDocument = false;
                    break;

                case TaskDialogResult.No:
                    _wholeDocument = true;
                    break;

                default:
                    return Result.Cancelled;
            }

            _app.Idling += OnIdle;

            return Result.Succeeded;
        }

        private void OnIdle(object sender, IdlingEventArgs e)
        {
            if (!_isRunning)
            {
                _app.Idling -= OnIdle;
            }

            if (_isRunning)
            {
                try
                {
                    Reference refTag = _uidoc.Selection.PickObject(ObjectType.Element, new SelectionFilter(), "Selecteer een pipe of duct tag");
                    IndependentTag tagElement = (IndependentTag)_doc.GetElement(refTag);
                    BuiltInCategory bic = (BuiltInCategory)tagElement.Category.Id.IntegerValue;

                    Transaction t = new Transaction(_doc, "change tag color");
                    t.Start();

                    if (_wholeDocument)
                    {
                        IEnumerable<View> viewCollector = new FilteredElementCollector(_doc)
                        .OfClass(typeof(View))
                        .Cast<View>();

                        foreach (View view in viewCollector)
                        {
                            if (view.IsTemplate)
                            {
                                continue;
                            }

                            ChangeTagColorInView(view, bic);
                        }
                    }

                    else
                    {
                        View view = (View)_doc.GetElement(tagElement.OwnerViewId);
                        ChangeTagColorInView(view, bic);
                    }

                    t.Commit();
                }

                catch (Exception)
                {
                    _isRunning = false;
                }
            }
        }
        private void ChangeTagColorInView(View view, BuiltInCategory bic)
        {
            IEnumerable<IndependentTag> allTagsInview = new FilteredElementCollector(_doc, view.Id)
                .WhereElementIsNotElementType()
                .OfCategory(bic)
                .Cast<IndependentTag>();

            foreach (IndependentTag tag in allTagsInview)
            {
                Element element = tag.GetTaggedLocalElements().First();
                if (element == null)
                {
                    continue;
                }

                ElementId pstId;

                try
                {
                    pstId = element.GetParameters("System Type").FirstOrDefault().AsElementId();
                }
                catch (Exception)
                {
                    string systemName = element.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM).AsString();

                    if (string.IsNullOrWhiteSpace(systemName))
                    {
                        continue;
                    }

                    string systemTypeName = systemName.Substring(0, systemName.LastIndexOf(' '));

                    try
                    {
                        pstId = new FilteredElementCollector(_doc)
                                        .OfClass(typeof(MEPSystemType))
                                        .Cast<MEPSystemType>()
                                        .FirstOrDefault(m => m.Name == systemTypeName)
                                        .Id;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                MEPSystemType mst = (MEPSystemType)_doc.GetElement(pstId);
                Color mstColor = mst.LineColor;

                OverrideGraphicSettings overrideSettings = new OverrideGraphicSettings();
                overrideSettings.SetProjectionLineColor(mstColor);

                view.SetElementOverrides(tag.Id, overrideSettings);
            }
        }
    }


    public class SelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category == null)
            {
                return false;
            }

            return elem.Category.CategoryType == CategoryType.Annotation && elem.Category.Name.Contains("Tags")
                && (elem.Category.Name.Contains("Duct") || elem.Category.Name.Contains("Pipe") || elem.Category.Name.Contains("Air Terminal")
                || elem.Category.Name.Contains("Mechanical Equipment"));
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
