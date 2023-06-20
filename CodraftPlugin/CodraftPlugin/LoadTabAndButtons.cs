using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace CodraftPlugin_Loading
{
    public class LoadTabAndButtons : IExternalApplication
    {
        private string assemblyPath = Assembly.GetExecutingAssembly().Location;
        public void AddRibbonPanel(UIControlledApplication app)
        {
           //Create Tab
            string tabName = "Gilles";
            app.CreateRibbonTab(tabName);

            //Create a ribbonpanels
            RibbonPanel updaters = app.CreateRibbonPanel(tabName, "Updaters");

            //Create pipeupdater button
            PushButtonData pipeUpdaterData = new PushButtonData(
                "pipeUpdater",
                "Pipe Updater",
                assemblyPath,
                "CodraftPlugin.EnableDisablePipeUpdater");

            //add button to ribbon + tooltip and image
            PushButton pipeUpdaterButton = updaters.AddItem(pipeUpdaterData) as PushButton;
            pipeUpdaterButton.ToolTip = "Enables/Disables the pipeupdater.";
            pipeUpdaterButton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin;component/Resources/PipeUpdater.png"));

        }

        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                UpdaterManager.UnregisterPipeUpdater(application.ActiveAddInId);

                return Result.Succeeded;
            }

            catch (Exception)
            {
                return Result.Failed;
            }
        }

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                AddRibbonPanel(application);
                UpdaterManager.RegisterPipeUpdater(application.ActiveAddInId);

                return Result.Succeeded;
            }

            catch (Exception)
            {

                return Result.Failed;
            }
        }
    }
}
