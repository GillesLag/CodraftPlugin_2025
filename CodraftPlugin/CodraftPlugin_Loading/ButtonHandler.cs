using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace CodraftPlugin_Loading
{
    public class ButtonHandler
    {
        /// <summary>
        /// Enables or disables the button that the user clicked on.
        /// </summary>
        /// <param name="up"></param>
        /// <param name="updaterName"></param>
        /// <param name="pngFile"></param>
        /// <param name="uiapp"></param>
        public static void EnableDisable(IUpdater up, string updaterName, string pngFile, UIApplication uiapp)
        {
            // Get list of all panels from the Gilles tab
            List<RibbonPanel> panels = uiapp.GetRibbonPanels("Codraft");
            // Get the Updaters tab
            RibbonPanel updater = panels.Where(r => r.Name == "Updaters").First();
            // Get the desired pushbutton
            PushButton pipeAccessoryUpdater = (PushButton)updater.GetItems().Where(r => r.Name == updaterName).First();

            if (!UpdaterRegistry.IsUpdaterEnabled(up.GetUpdaterId()))
            {
                // Enables updater and set image.
                UpdaterRegistry.EnableUpdater(up.GetUpdaterId());
                pipeAccessoryUpdater.LargeImage = new BitmapImage(new Uri($"pack://application:,,,/CodraftPlugin_Loading;component/Resources/{pngFile}On.png"));
            }

            else
            {
                // Disable updater and set image.
                UpdaterRegistry.DisableUpdater(up.GetUpdaterId());
                pipeAccessoryUpdater.LargeImage = new BitmapImage(new Uri($"pack://application:,,,/CodraftPlugin_Loading;component/Resources/{pngFile}.png"));
            }
        }
    }
}
