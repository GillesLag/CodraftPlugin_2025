using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using CodraftPlugin_DAL;
using CodraftPlugin_Library;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;

namespace CodraftPlugin_Updaters
{
    public class Insulation : IUpdater
    {
        //const float feetToMm = 304.8f;

        private Guid _guid = new Guid("DC47C2BD-17D8-4C21-8216-582CA8C543D7");
        private string globalParameterName = "RevitProjectMap";
        public UpdaterId Id { get; set; }

        public Insulation(AddInId addInId)
        {
            this.Id = new UpdaterId(addInId, _guid);
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            string projectMapPath = GlobalParameters.GetGlobalParameter(doc, "RevitProjectMap");
            string databasesMapPath = projectMapPath + @"\RevitTextFiles\Isolatie.xlsx";

            foreach (ElementId elemId in data.GetAddedElementIds())
            {
                try
                {
                    // Get all the info from the pipe element.
                    Pipe pipe = doc.GetElement(elemId) as Pipe;
                    ElementId elementId = elemId;

                    if (pipe == null)
                    {
                        PipeInsulation insulation = (PipeInsulation)doc.GetElement(elemId);

                        if (insulation == null)
                            continue;

                        elementId = insulation.HostElementId;
                        pipe = doc.GetElement(elementId) as Pipe;
                    }

                    if (pipe == null)
                        continue;

                    if (pipe.LookupParameter("COD_Isolatie").AsInteger() == 0)
                        continue;

                    string systemType = pipe.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString();
                    char[] numbersCharArray = pipe.get_Parameter(BuiltInParameter.RBS_CALCULATED_SIZE).AsString().TakeWhile(x => int.TryParse(x.ToString(), out _)).ToArray();
                    int nominaleDiameter = int.Parse(new string(numbersCharArray));

                    DataTable excelData = Utilities.GetInsulationData(doc);

                    string query = $"SELECT *" +
                    $" FROM IsolatieTabel" +
                    $" WHERE Medium = \"{systemType}\"" +
                    $" AND Nominale_diameter = {nominaleDiameter};";

                    // First object is string and second is a double.
                    List<object> lookupValues = FileOperations.LookupInsulation(query, databasesMapPath);

                    // there is no insulation if lookupValues is null.
                    if (lookupValues == null)
                        continue;

                    // Set the correct insulation type and thickness.
                    ElementSettings.ApplyInsulation(elementId, lookupValues, doc);

                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Insulation added element Error", ex.Message);
                    continue;
                }
            }

            foreach (ElementId elemId in data.GetModifiedElementIds())
            {
                try
                {
                    // Get all the info from the pipe element
                    ElementId elementId = elemId;

                    if (doc.GetElement(elemId) is not Pipe pipe)
                    {
                        PipeInsulation insulation = (PipeInsulation)doc.GetElement(elemId);
                        elementId = insulation.HostElementId;
                        pipe = doc.GetElement(elementId) as Pipe;
                    }

                    if (pipe == null)
                        continue;

                    if (pipe.LookupParameter("COD_Isolatie").AsInteger() == 0)
                        continue;

                    string systemType = pipe.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString();
                    int nominaleDiameter = int.Parse(pipe.get_Parameter(BuiltInParameter.RBS_CALCULATED_SIZE).AsString().Split(' ').First());

                    string query = $"SELECT *" +
                        $" FROM IsolatieTabel" +
                        $" WHERE Medium = \"{systemType}\"" +
                        $" AND Nominale_diameter = {nominaleDiameter};";

                    // First object is string and second is a double.
                    List<object> lookupValues = FileOperations.LookupInsulation(query, databasesMapPath);

                    // there is no insulation if lookupValues is null.
                    if (lookupValues == null)
                        continue;

                    // if the same insulation is already applied ignore and continue.
                    if (pipe.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_TYPE).HasValue && ElementSettings.IsPipeInsulationApplied(pipe, lookupValues))
                        continue;

                    ElementSettings.ApplyInsulation(elementId, lookupValues, doc);
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Insulation Modified element Error", ex.Message);
                    continue;
                }
            }
        }

        public string GetAdditionalInformation()
        {
            return "InsulationUpdater";
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
            return "InsulationUpdater";
        }
    }
}
