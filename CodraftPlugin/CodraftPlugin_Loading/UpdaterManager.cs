using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using CodraftPlugin_Updaters;

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

            UpdaterRegistry.AddTrigger(id,
                new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves),
                Element.GetChangeTypeParameter(new ElementId(BuiltInParameter.RBS_START_OFFSET_PARAM)));

            UpdaterRegistry.AddTrigger(id,
                new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves),
                Element.GetChangeTypeParameter(new ElementId(BuiltInParameter.RBS_END_OFFSET_PARAM)));

            UpdaterRegistry.AddTrigger(id,
                new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves),
                Element.GetChangeTypeParameter(new ElementId(BuiltInParameter.RBS_PIPE_OUTER_DIAMETER)));

            UpdaterRegistry.AddTrigger(id,
                new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves),
                Element.GetChangeTypeParameter(new ElementId(BuiltInParameter.ELEM_TYPE_PARAM)));

            UpdaterRegistry.AddTrigger(id,
                new ElementCategoryFilter(BuiltInCategory.OST_PipeCurves),
                Element.GetChangeTypeParameter(new ElementId(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM)));

            UpdaterRegistry.DisableUpdater(id);
        }

        public static void UnregisterPipeUpdater(AddInId addinId)
        {
            Pipes pipeUpdater = new Pipes(addinId);
            UpdaterId id = pipeUpdater.GetUpdaterId();

            UpdaterRegistry.RemoveAllTriggers(id);
            UpdaterRegistry.UnregisterUpdater(id);
        }

        public static void RegisterFittingUpdater(AddInId addinId)
        {
            Fittings fittingUpdater = new Fittings(addinId);
            UpdaterId id = fittingUpdater.GetUpdaterId();

            UpdaterRegistry.RegisterUpdater(fittingUpdater);

            UpdaterRegistry.AddTrigger(id,
                new ElementCategoryFilter(BuiltInCategory.OST_PipeFitting),
                Element.GetChangeTypeElementAddition());

            UpdaterRegistry.AddTrigger(id,
                new ElementCategoryFilter(BuiltInCategory.OST_PipeFitting),
                Element.GetChangeTypeAny());

            UpdaterRegistry.DisableUpdater(id);
        }

        public static void UnregisterFittingUpdater(AddInId addinId)
        {
            Fittings fittingUpdater = new Fittings(addinId);
            UpdaterId id =fittingUpdater.GetUpdaterId();

            UpdaterRegistry.RemoveAllTriggers(id);
            UpdaterRegistry.UnregisterUpdater(id);
        }
    }
}
