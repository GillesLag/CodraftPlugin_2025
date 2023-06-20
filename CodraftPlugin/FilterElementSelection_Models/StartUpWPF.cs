using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using FilterElementSelection_WPF;

namespace FilterElementSelection_Models
{
    [Transaction(TransactionMode.Manual)]
    public class StartUpWPF : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            
        }
    }
}
