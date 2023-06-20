using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using FilterElementSelection_WPF;
using System.Windows.Interop;
using System.Diagnostics;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class OpenFilterElementSelection : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MainWindow window = new MainWindow(commandData.Application.ActiveUIDocument);

            JtWindowHandle h = new JtWindowHandle(commandData.Application.MainWindowHandle);
            WindowInteropHelper helper = new WindowInteropHelper(window)
            {
                Owner = h.Handle
            };

            window.Show();

            return Result.Succeeded;
        }
    }
}
