using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodraftPlugin_Updaters;
using System;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class EnableDisablePipeUpdater : IExternalCommand
    {
        /// <summary>
        /// Enables or disables the pipe updater
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
                Pipes pUpdater = new Pipes(uiapp.ActiveAddInId);
                ButtonHandler.EnableDisable(pUpdater, "pipeUpdater", "PipeUpdater", uiapp);

                return Result.Succeeded;
            }

            catch (Exception)
            {

                return Result.Failed;
            }
        }
    }
}
