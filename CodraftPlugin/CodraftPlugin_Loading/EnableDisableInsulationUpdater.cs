using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodraftPlugin_Updaters;
using System;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class EnableDisableInsulationUpdater : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;

            try
            {
                Insulation insulUpdater = new Insulation(uiapp.ActiveAddInId);
                ButtonHandler.EnableDisable(insulUpdater, "insulationUpdater", "InsulationUpdater", uiapp);

                return Result.Succeeded;
            }
            catch (Exception)
            {
                return Result.Failed;
            }
        }
    }
}
