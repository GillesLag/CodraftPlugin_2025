using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodraftPlugin_Updaters;
using System;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class EnableDisableFittingUpdater : IExternalCommand
    {
        /// <summary>
        /// Enables or disables the fitting updater.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;

            try
            {
                Fittings fUpdater = new Fittings(uiapp.ActiveAddInId);
                ButtonHandler.EnableDisable(fUpdater, "fittingUpdater", "FittingUpdater", uiapp);

                return Result.Succeeded;
            }

            catch (Exception)
            {

                return Result.Failed;
            }
        }
    }
}
