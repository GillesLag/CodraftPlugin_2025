using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodraftPlugin_DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using CodraftPlugin_Library;
using System.Security.Principal;
using Autodesk.Revit.ApplicationServices;
using CodraftPlugin_Updaters;

namespace CodraftPlugin_Loading
{
    [Transaction(TransactionMode.Manual)]
    public class OpstartWerkomgeving : IExternalCommand
    {
        private const float feetToMm = 304.8f;

        private string materialQuery = "SELECT * FROM Materiaal";
        private string scheduleQuery = "SELECT * FROM Schedule";
        private string SystemtypeQuery = "SELECT * FROM SystemTypes";
        private string pipeTypeQuery = "SELECT * FROM PipeTypes";
        private string joinSegmentAndSizesQuery = "SELECT * FROM SegmentSize";
        private string insulMaterialQuery = "SELECT * FROM IsolatieMateriaal";
        private string connection = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\"";
        private readonly string globalParameterName = "RevitProjectMap";
        private bool fUpdaterChanged = false;
        private bool pUpdaterChanged = false;
        private bool iUpdaterChanged = false;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            Application app = doc.Application;

            //Check if the updaters are active
            Fittings fUpdater = new Fittings(uiapp.ActiveAddInId);
            Pipes pUpdater = new Pipes(uiapp.ActiveAddInId);
            Insulation iUpdater = new Insulation(uiapp.ActiveAddInId);

            if (UpdaterRegistry.IsUpdaterEnabled(fUpdater.GetUpdaterId()))
            {
                ButtonHandler.EnableDisable(fUpdater, "fittingUpdater", "FittingUpdater", uiapp);
                fUpdaterChanged = true;
            }

            if (UpdaterRegistry.IsUpdaterEnabled(pUpdater.GetUpdaterId()))
            {
                ButtonHandler.EnableDisable(pUpdater, "pipeUpdater", "PipeUpdater", uiapp);
                pUpdaterChanged = true;
            }

            if (UpdaterRegistry.IsUpdaterEnabled(iUpdater.GetUpdaterId()))
            {
                ButtonHandler.EnableDisable(iUpdater, "insulationUpdater", "InsulationUpdater", uiapp);
                iUpdaterChanged = true;
            }

            string pathProject;
            string pathDatabase;

            // Check globalparameter for projectfoldermap
            ElementId globalParameter = GlobalParametersManager.FindByName(doc, globalParameterName);

            if (globalParameter == ElementId.InvalidElementId)
            {
                Transaction t = new Transaction(doc, "Globalparameter instellen");
                t.Start();

                pathProject = GlobalParameters.SetGlobalParameter(doc, globalParameterName);

                t.Commit();
            }
            else
            {
                GlobalParameter revitProjectMapParameter = (GlobalParameter)doc.GetElement(globalParameter);
                pathProject = ((StringParameterValue)revitProjectMapParameter.GetValue()).Value;
            }

            if (pathProject.Contains("(user)"))
            {
                string username = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                pathProject = pathProject.Replace("(user)", username);
            }

            pathDatabase = pathProject + @"\RevitDatabases\OpstartWerkomgeving_Revit.accdb";
            connection += pathDatabase + "\"";


            // Add all projectparameters
            Transaction addParameters = new Transaction(doc, "addProjectParameters");

            app.SharedParametersFilename = pathProject + @"\RevitTextFiles\COD_project_parameters.txt";
            DefinitionFile defFile = app.OpenSharedParameterFile();

            var pipeProjectParameters = defFile.Groups.First(g => g.Name == "Constraints_Piping");
            var codIsolatieProjectParameter = defFile.Groups.First(g => g.Name == "Other_Fittings/Piping");
            var categorySet = new CategorySet();
            categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeCurves));

            addParameters.Start();
            foreach (var item in pipeProjectParameters.Definitions)
            {
                Binding binding = app.Create.NewInstanceBinding(categorySet);
                BindingMap map = doc.ParameterBindings;
                map.Insert(item, binding, BuiltInParameterGroup.PG_CONSTRAINTS);
            }

            categorySet.Insert(Category.GetCategory(doc, BuiltInCategory.OST_PipeFitting));

            foreach (var item in codIsolatieProjectParameter.Definitions)
            {
                Binding binding = app.Create.NewInstanceBinding(categorySet);
                BindingMap map = doc.ParameterBindings;
                map.Insert(item, binding, BuiltInParameterGroup.PG_INSULATION);
            }
            addParameters.Commit();

            // Add COD families
            Transaction famTran = new Transaction(doc, "Add Families");
            string bocht = pathProject + @"\RevitFamilies\COD_50_PIF_UN_Bocht_gen_BERSnl.rfa";
            string cap = pathProject + @"\RevitFamilies\COD_50_PIF_UN_Cap_gen_BERSnl.rfa";
            string reductie = pathProject + @"\RevitFamilies\COD_50_PIF_UN_Reductie_gen_BERSnl.rfa";
            string tstuk = pathProject + @"\RevitFamilies\COD_50_PIF_UN_T_Stuk_gen_BERSnl.rfa";
            string tap = pathProject + @"\RevitFamilies\COD_50_PIF_UN_Tap_gen_BERSnl.rfa";
            string[] allCodFamilies = new string[] { bocht, cap, reductie, tap, tstuk };

            famTran.Start();

            foreach (string family in allCodFamilies)
            {
                doc.LoadFamily(family);
            }

            famTran.Commit();

            // Add all materials
            Transaction matTrans = new Transaction(doc, "AddMaterials");
            matTrans.Start();

            foreach (string i in FileOperations.GetMaterials(materialQuery, connection))
            {
                try
                {
                    Material.Create(doc, i);
                }
                catch (Exception)
                {
                    continue;
                }

            }

            foreach (string i in FileOperations.GetInsulationMaterials(insulMaterialQuery, connection))
            {
                try
                {
                    Material.Create(doc, i);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            matTrans.Commit();



            // Add all schedules
            Transaction scheduleTrans = new Transaction(doc, "AddPipeSchedules");
            scheduleTrans.Start();

            foreach (string i in FileOperations.GetSchedules(scheduleQuery, connection))
            {
                try
                {
                    PipeScheduleType.Create(doc, i);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            scheduleTrans.Commit();



            // Get a table of al the segments with its sizes
            List<List<object>> SegmentAndSizelist = FileOperations.GetSegmentsAndSizeList(joinSegmentAndSizesQuery, connection);

            string scheduleType = "";
            List<MEPSize> mepSizes = new List<MEPSize>();
            string mat = "";

            // Loop through the table and get for every id a list of mepsizes.
            int indexSegmentAndSizelist = 1;
            foreach (List<object> row in SegmentAndSizelist)
            {
                string newSchedulType = (string)row[0];
                string newMat = (string)row[1];
                if (scheduleType != newSchedulType || mat != newMat || indexSegmentAndSizelist == SegmentAndSizelist.Count)
                {
                    if (indexSegmentAndSizelist == SegmentAndSizelist.Count)
                    {
                        MEPSize lastMs = new MEPSize((double)row[2] / feetToMm, (double)row[3] / feetToMm, (double)row[4] / feetToMm, true, true);
                        mepSizes.Add(lastMs);
                    }
                    while (mepSizes.Count != 0)
                    {
                        PipeScheduleType pst = new FilteredElementCollector(doc)
                            .OfClass(typeof(PipeScheduleType))
                            .Cast<PipeScheduleType>()
                            .Single(x => x.Name == scheduleType);

                        Material material = new FilteredElementCollector(doc)
                            .OfClass(typeof(Material))
                            .Cast<Material>()
                            .Single(x => x.Name == mat);

                        // Add all the segments
                        Transaction t = new Transaction(doc, "AddPipeSegment");

                        try
                        {
                            t.Start();
                            PipeSegment.Create(doc, material.Id, pst.Id, mepSizes);
                            t.Commit();
                        }
                        catch (Exception)
                        {
                            t.Commit();
                            mepSizes.Clear();
                            continue;
                        }
                    }

                    scheduleType = (string)row[0];
                    mat = (string)row[1];
                }

                double insideDiameterDatabase = (double)row[3];
                double insideDiameter = insideDiameterDatabase <= 0 ? 0.5 : insideDiameterDatabase;

                MEPSize ms = new MEPSize((double)row[2] / feetToMm, insideDiameter / feetToMm, (double)row[4] / feetToMm, true, true);
                mepSizes.Add(ms);
                indexSegmentAndSizelist++;
            }



            // Get all insulation materials
            List<string> insulMatList = FileOperations.GetInsulationMaterials(insulMaterialQuery, connection);

            // Get a PipeInsulationType to duplicate
            PipeInsulationType pit = new FilteredElementCollector(doc)
                .OfClass(typeof(PipeInsulationType))
                .Cast<PipeInsulationType>()
                .First();

            foreach (string i in insulMatList)
            {
                // Get the material from the document
                Material insulMat = new FilteredElementCollector(doc)
                    .OfClass(typeof(Material))
                    .Cast<Material>()
                    .Single(x => x.Name == i);

                // Add the insulationType to the document.
                Transaction insulTypeTrans = new Transaction(doc, "AddInsulationType");


                try
                {
                    insulTypeTrans.Start();
                    ElementId elemId = pit.Duplicate(i).Id;
                    PipeInsulationType newType = (PipeInsulationType)doc.GetElement(elemId);
                    newType.get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM).Set(insulMat.Id);
                    insulTypeTrans.Commit();
                }
                catch (Exception)
                {
                    insulTypeTrans.Commit();
                    continue;
                }

            }

            Transaction deletePit = new Transaction(doc, "Delete orginal insulaitonType");
            deletePit.Start();
            // Delete orginal insulationType.
            doc.Delete(pit.Id);

            deletePit.Commit();



            // Get the table of the all the systemTypes
            List<List<string>> systemTypes = FileOperations.GetSystemTypes(SystemtypeQuery, connection);

            // Loop over all the systemTypes
            foreach (List<string> i in systemTypes)
            {
                List<byte> col = i[2].Split(',').Select(x => byte.Parse(x)).ToList();
                Color revitColor = new Color(col[0], col[1], col[2]);
                string name = i[0];
                string typeTemplate = i[1];
                string abr = i[3];
                MEPSystemClassification msc;

                PipingSystemType psType = new FilteredElementCollector(doc)
                    .OfClass(typeof(PipingSystemType))
                    .Cast<PipingSystemType>()
                    .FirstOrDefault(x => x.Name == name);

                if (psType != null)
                {
                    continue;
                }

                // Set the MEPSystemClassification
                switch (typeTemplate)
                {
                    case "Domestic Cold Water":
                        msc = MEPSystemClassification.DomesticColdWater;
                        break;

                    case "Domestic Hot Water":
                        msc = MEPSystemClassification.DomesticHotWater;
                        break;

                    case "Fire Protection Dry":
                        msc = MEPSystemClassification.FireProtectDry;
                        break;

                    case "Fire Protection Other":
                        msc = MEPSystemClassification.FireProtectOther;
                        break;

                    case "Fire Protection Pre-Action":
                        msc = MEPSystemClassification.FireProtectPreaction;
                        break;

                    case "Fire Protection Wet":
                        msc = MEPSystemClassification.FireProtectWet;
                        break;

                    case "Hydronic Return":
                        msc = MEPSystemClassification.ReturnHydronic;
                        break;

                    case "Hydronic Supply":
                        msc = MEPSystemClassification.SupplyHydronic;
                        break;

                    case "Other":
                        msc = MEPSystemClassification.OtherPipe;
                        break;

                    case "Sanitary":
                        msc = MEPSystemClassification.Sanitary;
                        break;

                    case "Vent":
                        msc = MEPSystemClassification.Vent;
                        break;

                    default:
                        msc = MEPSystemClassification.OtherPipe;
                        name = "FoutTijdensUitoerenCode";
                        break;
                }

                // Add SystemType
                Transaction systemTypeTrans = new Transaction(doc, "AddSystemType");

                try
                {
                    systemTypeTrans.Start();
                    PipingSystemType pst = PipingSystemType.Create(doc, msc, name);
                    pst.LineColor = revitColor;
                    pst.get_Parameter(BuiltInParameter.ALL_MODEL_URL).Set("www.codraft.be");
                    pst.TwoLineDropType = RiseDropSymbol.YinYangFilled;
                    pst.TwoLineRiseType = RiseDropSymbol.YinYang;
                    pst.Abbreviation = abr;
                    systemTypeTrans.Commit();
                }
                catch (Exception)
                {
                    systemTypeTrans.Commit();
                    continue;
                }

            }



            // Get first pipeType to duplicate from
            PipeType pt = new FilteredElementCollector(doc)
                .OfClass(typeof(PipeType))
                .Cast<PipeType>()
                .First();

            // get CODFamilies familySymbols
            IEnumerable<FamilySymbol> fsFittings = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Where(x => x.Name == "Elbow" || x.Name == "Transition_Concentrisch" || x.Name == "Transition_Excentrisch" || x.Name == "Tee" || x.Name == "Tap" || x.Name == "Cap");

            List<List<object>> pipeTypeList = FileOperations.GetPipeTypes(pipeTypeQuery, connection);
            List<string> segmentList = new List<string>();
            List<(double minDn, double maxDn)> minMaxDnList = new List<(double, double)>();
            string ptName = "";
            string teeOrTap = "";
            string excenOrConcen = "";

            // Loop through all the pipeTypes
            for (int i = 0; i < pipeTypeList.Count; i++)
            {
                string newName = (string)pipeTypeList[i][1];
                if (newName.Contains("PIS"))
                {
                    newName = newName.Replace("PIS", "PI");
                }

                if (newName != ptName)
                {
                    PipeTypesToevoegen(segmentList, ptName, pt, fsFittings, minMaxDnList, teeOrTap, excenOrConcen, doc);

                    segmentList.Clear();
                    minMaxDnList.Clear();
                    ptName = newName;
                    teeOrTap = ((string)pipeTypeList[i][5]).ToLower();
                    excenOrConcen = ((string)pipeTypeList[i][6]).ToLower();
                }

                segmentList.Add((string)pipeTypeList[i][2]);
                minMaxDnList.Add(((double)pipeTypeList[i][3], (double)pipeTypeList[i][4]));
            }

            PipeTypesToevoegen(segmentList, ptName, pt, fsFittings, minMaxDnList, teeOrTap, excenOrConcen, doc);


            if (fUpdaterChanged)
            {
                ButtonHandler.EnableDisable(fUpdater, "fittingUpdater", "FittingUpdater", uiapp);
            }

            if (pUpdaterChanged)
            {
                ButtonHandler.EnableDisable(pUpdater, "pipeUpdater", "PipeUpdater", uiapp);
            }

            if (iUpdaterChanged)
            {
                ButtonHandler.EnableDisable(iUpdater, "insulationUpdater", "InsulationUpdater", uiapp);
            }

            return Result.Succeeded;
        }



        private void PipeTypesToevoegen(List<string> segmentList, string ptName, PipeType pt, IEnumerable<FamilySymbol> fsFittings, List<(double minDn, double maxDn)> minMaxDn, string teeOrTap, string excenOrConcen, Document doc)
        {
            for (int j = 0; j < segmentList.Count; j++)
            {
                // Get pipeSegment
                PipeSegment ps = new FilteredElementCollector(doc)
                    .OfClass(typeof(PipeSegment))
                    .Cast<PipeSegment>()
                    .Single(x => x.Name == segmentList[j]);

                // Create segment rule.
                RoutingPreferenceRule rprSegment = new RoutingPreferenceRule(ps.Id, "COD_" + segmentList[j]);
                rprSegment.AddCriterion(new PrimarySizeCriterion(minMaxDn[j].minDn / feetToMm, minMaxDn[j].maxDn / feetToMm));

                // Create fitting rules.
                RoutingPreferenceRule rprElbow = new RoutingPreferenceRule(fsFittings.Single(x => x.Name == "Elbow").Id, "elbow");
                rprElbow.AddCriterion(PrimarySizeCriterion.All());
                RoutingPreferenceRule rprCap = new RoutingPreferenceRule(fsFittings.Single(x => x.Name == "Cap").Id, "cap");
                rprCap.AddCriterion(PrimarySizeCriterion.All());

                RoutingPreferenceRule rprTee;
                RoutingPreferenceRule rprTran;

                if (teeOrTap == "tee")
                    rprTee = new RoutingPreferenceRule(fsFittings.Single(x => x.Name == "Tee").Id, "tee");
                else
                    rprTee = new RoutingPreferenceRule(fsFittings.Single(x => x.Name == "Tap").Id, "tap");

                if (excenOrConcen == "excentrisch")
                    rprTran = new RoutingPreferenceRule(fsFittings.Single(x => x.Name == "Transition_Excentrisch").Id, "excentrisch");
                else
                    rprTran = new RoutingPreferenceRule(fsFittings.Single(x => x.Name == "Transition_Concentrisch").Id, "concentrisch");

                rprTee.AddCriterion(PrimarySizeCriterion.All());
                rprTran.AddCriterion(PrimarySizeCriterion.All());

                // Start transation.
                Transaction ptTrans = new Transaction(doc, "AddPipeType");
                try
                {
                    ptTrans.Start();

                    // Create new pipetype.
                    PipeType nieuwPt;
                    if (j == 0)
                    {
                        nieuwPt = (PipeType)pt.Duplicate(ptName);
                    }
                    else
                    {
                        nieuwPt = new FilteredElementCollector(doc)
                            .OfClass(typeof(PipeType))
                            .Cast<PipeType>()
                            .Single(x => x.Name == ptName);
                    }

                    RoutingPreferenceManager rpm = nieuwPt.RoutingPreferenceManager;

                    // Add segment rules.
                    rpm.AddRule(RoutingPreferenceRuleGroupType.Segments, rprSegment, 0);

                    if (j == 0)
                    {
                        // Set junctionType.
                        if (teeOrTap == "tee")
                            rpm.PreferredJunctionType = PreferredJunctionType.Tee;
                        else
                            rpm.PreferredJunctionType = PreferredJunctionType.Tap;

                        // Add fitting rules.
                        rpm.AddRule(RoutingPreferenceRuleGroupType.Elbows, rprElbow, 0);
                        rpm.AddRule(RoutingPreferenceRuleGroupType.Caps, rprCap, 0);
                        rpm.AddRule(RoutingPreferenceRuleGroupType.Junctions, rprTee, 0);
                        rpm.AddRule(RoutingPreferenceRuleGroupType.Transitions, rprTran, 0);

                        // Delete default rule.
                        for (int i = 0; i < 5; i++)
                        {
                            int rules = 0;
                            switch (i)
                            {
                                case 0: rules = rpm.GetNumberOfRules(RoutingPreferenceRuleGroupType.Segments); break;
                                case 1: rules = rpm.GetNumberOfRules(RoutingPreferenceRuleGroupType.Elbows); break;
                                case 2: rules = rpm.GetNumberOfRules(RoutingPreferenceRuleGroupType.Caps); break;
                                case 3: rules = rpm.GetNumberOfRules(RoutingPreferenceRuleGroupType.Junctions); break;
                                case 4: rules = rpm.GetNumberOfRules(RoutingPreferenceRuleGroupType.Transitions); break;
                            }

                            for (int k = 1; k < rules; k++)
                            {
                                switch (i)
                                {
                                    case 0: rpm.RemoveRule(RoutingPreferenceRuleGroupType.Segments, 1); break;
                                    case 1: rpm.RemoveRule(RoutingPreferenceRuleGroupType.Elbows, 1); break;
                                    case 2: rpm.RemoveRule(RoutingPreferenceRuleGroupType.Caps, 1); break;
                                    case 3: rpm.RemoveRule(RoutingPreferenceRuleGroupType.Junctions, 1); break;
                                    case 4: rpm.RemoveRule(RoutingPreferenceRuleGroupType.Transitions, 1); break;
                                }
                            }
                        }
                    }

                    ptTrans.Commit();
                }
                catch (Exception ex)
                {
                    ptTrans.Commit();
                    continue;
                }

            }
        }
    }
}
