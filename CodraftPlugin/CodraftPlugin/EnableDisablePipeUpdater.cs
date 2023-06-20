using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Reflection;
using System.Windows.Media.Imaging;
using CodraftPlugin_Updaters;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class EnableDisablePipeUpdater : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;

            try
            {
                List<RibbonPanel> panels =  uiapp.GetRibbonPanels("Gilles");
                RibbonPanel updater = panels.Where(r => r.Name == "Updaters").First();
                PushButton pipeUpdater = (PushButton)updater.GetItems().Where(r => r.Name == "pipeUpdater").First();

                Pipes pUpdater = new Pipes(uiapp.ActiveAddInId);

                if (!UpdaterRegistry.IsUpdaterEnabled(pUpdater.GetUpdaterId()))
                {
                    UpdaterRegistry.EnableUpdater(pUpdater.GetUpdaterId());
                    pipeUpdater.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin;component/Resources/PipeUpdaterOn.png"));
                }

                else
                {
                    UpdaterRegistry.DisableUpdater(pUpdater.GetUpdaterId());
                    pipeUpdater.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin;component/Resources/PipeUpdater.png"));
                }

                return Result.Succeeded;
            }

            catch (Exception)
            {

                return Result.Failed;
            }
        }
    }
}
