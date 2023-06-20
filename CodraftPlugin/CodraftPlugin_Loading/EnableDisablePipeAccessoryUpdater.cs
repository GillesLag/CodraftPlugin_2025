using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodraftPlugin_Updaters;
using System;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class EnableDisablePipeAccessoryUpdater : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;

            try
            {
                PipeAccessories pUpdater = new PipeAccessories(uiapp.ActiveAddInId);
                ButtonHandler.EnableDisable(pUpdater, "pipeAccessoryUpdater", "PipeAccessory", uiapp);

                return Result.Succeeded;
            }

            catch (Exception)
            {

                return Result.Failed;
            }
        }
    }
}
