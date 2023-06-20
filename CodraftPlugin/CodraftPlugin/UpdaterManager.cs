using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodraftPlugin_Updaters;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace CodraftPlugin_Loading
{
    public class UpdaterManager
    {
        public static void RegisterPipeUpdater(AddInId addinId)
        {
            Pipes pipeUpdater = new Pipes(addinId);
            UpdaterId id = pipeUpdater.GetUpdaterId();

            UpdaterRegistry.RegisterUpdater(pipeUpdater);
            UpdaterRegistry.AddTrigger(id,
                new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves),
                Element.GetChangeTypeElementAddition());

            UpdaterRegistry.DisableUpdater(id);
        }

        public static void UnregisterPipeUpdater(AddInId addinId)
        {
            Pipes pipeUpdater = new Pipes(addinId);
            UpdaterId id = pipeUpdater.GetUpdaterId();

            UpdaterRegistry.RemoveAllTriggers(id);
            UpdaterRegistry.UnregisterUpdater(id);
        }
    }
}
