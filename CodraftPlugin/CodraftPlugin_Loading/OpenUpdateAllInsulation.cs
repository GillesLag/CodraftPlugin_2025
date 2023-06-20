using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using CodraftPlugin_UpdateAllInsulation;
using System.Windows.Interop;
using System.Diagnostics;
using System.Windows;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class OpenUpdateAllInsulation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Window win = new MainWindow(commandData.Application.ActiveUIDocument.Document);
            win.ShowDialog();

            return Result.Succeeded;
        }
    }
}
