using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Updaters;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace CodraftPlugin
{
    [Transaction(TransactionMode.Manual)]
    internal class PipeUpdater : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            AddInId addinId = uiapp.ActiveAddInId;
            
            try
            {
                Pipes test = new Pipes(addinId, doc);

                return Result.Succeeded;
            }

            catch (Exception)
            {
                return Result.Failed;
            }
        }
    }
}
