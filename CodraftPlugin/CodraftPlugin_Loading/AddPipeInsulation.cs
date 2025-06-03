using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using CodraftPlugin_Library;
using CodraftPlugin_Updaters;
using FilterDataGrid;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class AddPipeInsulation : IExternalCommand
    {
        private Document doc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            doc = commandData.Application.ActiveUIDocument.Document;
            DataTable data;

            if (!TryGetDataFromExcel(doc, out data))
            {
                return Result.Failed;
            }

            var allPipes = new FilteredElementCollector(doc)
                .OfClass(typeof(Pipe))
                .WhereElementIsNotElementType()
                .Cast<Pipe>();

            var allInsulationTypes = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PipeInsulations)
                .WhereElementIsElementType()
                .Cast<PipeInsulationType>();

            var allPipeSystems = new FilteredElementCollector(doc)
                .OfClass(typeof(PipingSystemType))
                .WhereElementIsElementType()
                .Cast<PipingSystemType>();

            Transaction t = new Transaction(doc, "Add Insulation To Pipes");
            t.Start();

            var missingPipeSystemTypes = new List<string>();
            var missingInsulationTypes = new List<string>();
            var rows = data.Rows;

            for (int i = 0; i < rows.Count; i++)
            {
                var isInsualtionOrPipeSystemMissing = false;
                var rowData = rows[i].ItemArray;

                string systemType = (string)rowData[0];
                double diameter = UnitUtils.Convert((double)rowData[3], UnitTypeId.Millimeters, UnitTypeId.Feet);
                string insulationType = (string)rowData[1];
                double insulationThickness = UnitUtils.Convert((double)rowData[2], UnitTypeId.Millimeters, UnitTypeId.Feet);

                if (!allPipeSystems.Any(ps => ps.Name == systemType))
                {
                    isInsualtionOrPipeSystemMissing = true;
                    if (!missingPipeSystemTypes.Contains(systemType))
                    {
                        missingPipeSystemTypes.Add(systemType);
                    }
                }

                var insulation = allInsulationTypes.FirstOrDefault(x => x.Name == insulationType);
                if (insulation == null)
                {
                    isInsualtionOrPipeSystemMissing = true;
                    if (!missingInsulationTypes.Contains(insulationType))
                    {
                        missingInsulationTypes.Add(insulationType);
                    }
                }

                if (isInsualtionOrPipeSystemMissing)
                {
                    continue;
                }

                var pipes = allPipes.Where(p => p.LookupParameter("System Type").AsValueString() == systemType &&
                    p.Diameter == diameter);

                ApplyInsulationForPipes(doc, pipes, insulationThickness, insulation);
            }

            t.Commit();

            if (missingPipeSystemTypes.Any() || missingInsulationTypes.Any())
            {
                var dialog = new TaskDialog("Add Pipe Insulation");
                dialog.MainInstruction = "PipeSystemType and/or InsulationType is missing. see below for more details.";
                dialog.ExpandedContent = $"PIPESYSTEM TYPES MISSING: {MissingItemsToString(missingPipeSystemTypes)}\n\nINSULATION TYPES MISSING: {MissingItemsToString(missingInsulationTypes)}";
                dialog.Show();

                return Result.Succeeded;
            }

            TaskDialog.Show("Add Pipe Insulation", "Insulation Added Successfully");

            return Result.Succeeded;
        }

        private void ApplyInsulationForPipes(Document doc, IEnumerable<Pipe> pipes, double insulationThickness, PipeInsulationType insulation)
        {
            foreach (var pipe in pipes)
            {
                var elemId = pipe.GetDependentElements(new ElementClassFilter(typeof(PipeInsulation))).FirstOrDefault();
                if (elemId != null)
                {
                    var insul = doc.GetElement(elemId) as PipeInsulation;
                    insul.Thickness = insulationThickness;
                    var test = insul.GetParameter(ParameterTypeId.ElemTypeParam);
                    insul.GetParameter(ParameterTypeId.ElemTypeParam).Set(insulation.Id);
                    continue;
                }
                PipeInsulation.Create(doc, pipe.Id, insulation.Id, insulationThickness);
            }
        }

        private bool TryGetDataFromExcel(Document doc, out DataTable data)
        {
            data = null;

            try
            {
                data = Utilities.GetInsulationData(doc);
            }
            catch (FileNotFoundException ex)
            {
                TaskDialog.Show("Insulation file not found", "Check the RevitTextFiles folder for the excel file");
            }
            catch (IOException ex)
            {
                TaskDialog.Show("Please Close Excel File", "The Excel file is open, Please close it.");
                return false;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
                return false;
            }

            return true;
        }

        private string MissingItemsToString(List<string> items)
        {
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                if (sb.Length != 0)
                {
                    sb.Append(", ");
                }
                sb.Append(item);
            }

            return sb.ToString();
        }
        
    }
}
