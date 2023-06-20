using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Plumbing;
using System.Reflection;
using System.Windows.Forms;
using CodraftPlugin_Settings;

namespace CodraftPlugin_Updaters
{
    public class Pipes : IUpdater
    {
        private Guid _guid = new Guid("119D4855-D967-4DD0-AE69-0DB8B0C06296");
        public UpdaterId Id { get; set; }
        public Pipes(AddInId addinId)
        {
            this.Id = new UpdaterId(addinId, _guid);
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();

            // Lijst van alle pipesystems namen.
            List<PipingSystemType> pipingSystems = new FilteredElementCollector(doc)
                .OfClass(typeof(PipingSystemType))
                .Cast<PipingSystemType>()
                .ToList();

            List<ElementId> systemIds = pipingSystems.Select(x => x.Id).ToList();
            List<string> systemNames = pipingSystems.Select(x => x.Name).ToList();

            // Geef voor elke pipe de juiste systemtype.
            foreach (ElementId pipeId in data.GetAddedElementIds())
            {
                // Haal het pipe element uit het document.
                Pipe pipe = (Pipe)doc.GetElement(pipeId);

                // Haal de systemnaam uit de pipetype naam
                string systemName = pipe.Name.Split('%').First();

                // Als de systemtype niet bestaat delete de pipe.
                if (!systemNames.Contains(systemName))
                {
                    doc.Delete(pipeId);
                    SendKeys.SendWait("{ESC}");

                    // Melding maken voor gebruiker.
                    TaskDialog td = new TaskDialog("PipeType of PipingSystem Fout");
                    td.MainInstruction = "De PipeType naam komt niet overeen met een PipingSystem naam";
                    td.ExpandedContent = "De PipeType naam moet overeenkomen met de PipingSystem naam\n" +
                        "voorbeeld PipeType naam: \'KW_Retour\'.\n" +
                        "Als je extra info in de PipeType naam wilt doe je dit door een \'%\' symbool\n toe te voegen." +
                        "Voorbeeld: \'KW_Retour%extra info...\'";
                    td.Show();
                    
                    continue;
                }

                // Stelt de juiste systemtype in voor de pipe.
                ElementSettings.SetParamtersPipe(pipe, systemIds[systemNames.IndexOf(systemName)]);
            }

            foreach (ElementId pipeId in data.GetModifiedElementIds())
            {
                TaskDialog.Show("test", "test");
            }
        }

        public string GetAdditionalInformation()
        {
            return "PipesUpdater";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.MEPAccessoriesFittingsSegmentsWires;
        }

        public UpdaterId GetUpdaterId()
        {
            return Id;
        }

        public string GetUpdaterName()
        {
            return "PipeUpdater";
        }
    }
}
