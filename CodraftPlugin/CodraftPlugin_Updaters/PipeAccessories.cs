using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using CodraftPlugin_DAL;
using CodraftPlugin_Exceptions;
using CodraftPlugin_Library;
using CodraftPlugin_Updaters.PipeAccessoriesTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace CodraftPlugin_Updaters
{
    public class PipeAccessories : IUpdater
    {
        private Guid _guid = new Guid("41494CDF-1377-434D-B8A5-C7D6A148D889");
        private List<ElementId> _familySubelementIds = new List<ElementId>();
        private List<ElementId> _updatedElementids = new List<ElementId>();
        private string _pipeAccessoryName;
        private Guid failureGuidPipeAccessories = new Guid("4B81D4C5-185C-4830-8ECF-67370ADB06B0");
        private string globalParameterName = "RevitProjectMap";

        public UpdaterId Id { get; set; }

        public PipeAccessories(AddInId addinId)
        {
            this.Id = new UpdaterId(addinId, _guid);
        }

        public void Execute(UpdaterData data)
        {
            Document doc = data.GetDocument();
            string projectMapPath;
            string databasesMapPath;
            string textFilesMapPath;

            ElementId globalParameter = GlobalParametersManager.FindByName(doc, globalParameterName);

            if (globalParameter == ElementId.InvalidElementId)
            {
                projectMapPath = GlobalParameters.SetGlobalParameter(doc, globalParameterName);
            }
            else
            {
                GlobalParameter revitProjectMapParameter = (GlobalParameter)doc.GetElement(globalParameter);
                projectMapPath = ((StringParameterValue)revitProjectMapParameter.GetValue()).Value;
            }

            if (projectMapPath.Contains("(user)"))
            {
                string username = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                projectMapPath = projectMapPath.Replace("(user)", username);
            }

            databasesMapPath = projectMapPath + @"\RevitDatabases\";
            textFilesMapPath = projectMapPath + @"\RevitTextFiles\";

            FailureDefinitionId warning = new FailureDefinitionId(failureGuidPipeAccessories);
            FailureMessage fm = new FailureMessage(warning);

            foreach (ElementId id in data.GetAddedElementIds())
            {
                if (!(doc.GetElement(id) is FamilyInstance pipeAccessory))
                    return;

                _pipeAccessoryName = pipeAccessory.Symbol.FamilyName;

                if (!_pipeAccessoryName.Contains("COD"))
                    continue;

                IEnumerable<ElementId> subElementTypeIds = pipeAccessory.GetSubComponentIds().Select(x => ((FamilyInstance)doc.GetElement(x)).GetTypeId());
                _familySubelementIds.AddRange(subElementTypeIds);

                if (_familySubelementIds.Contains(pipeAccessory.GetTypeId()))
                {
                    _familySubelementIds.Remove(pipeAccessory.GetTypeId());
                    continue;
                }

                _updatedElementids.Add(pipeAccessory.Id);

                try
                {
                    bool? hasParams;
                    switch (_pipeAccessoryName)
                    {
                        case "COD_KOGELKRAAN":

                            StraightValve straightValve = new StraightValve(pipeAccessory, doc, databasesMapPath);

                            hasParams = straightValve.GetParams();

                            if (hasParams == false)
                            {
                                doc.PostFailure(fm);
                                straightValve.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (straightValve.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParamtersStraightValve(straightValve.DatabaseParameters, straightValve.PipeAccessory);
                            }

                            break;

                        case "COD_INREGELAFSLUITER":

                            BalanceValve balanceValve = new BalanceValve(pipeAccessory, doc, databasesMapPath);

                            hasParams = balanceValve.GetParams();

                            if(hasParams == false)
                            {
                                doc.PostFailure(fm);
                                balanceValve.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (balanceValve.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParametersBalanceValve(balanceValve.DatabaseParameters, balanceValve.PipeAccessory);
                            }

                            break;

                        case "COD_VLINDERKLEP":

                            ButterflyValve butterflyValve = new ButterflyValve(pipeAccessory, doc, databasesMapPath);

                            hasParams = butterflyValve.GetParams();

                            if (hasParams == false)
                            {
                                doc.PostFailure(fm);
                                butterflyValve.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (butterflyValve.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParametersButterflyValve(butterflyValve.DatabaseParameters, butterflyValve.PipeAccessory);
                            }

                            break;

                        case "COD_Y-TYPEFILTER":

                            Strainer strainer = new Strainer(pipeAccessory, doc, databasesMapPath);

                            hasParams = strainer.GetParams();

                            if (hasParams == false)
                            {
                                doc.PostFailure(fm);
                                strainer.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (strainer.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParametersStrainer(strainer.DatabaseParameters, strainer.PipeAccessory);
                            }

                            break;

                        case "COD_3WEGGLOBEVALVE":

                            ThreeWayGlobeValve threeWayGlobeValve = new ThreeWayGlobeValve(pipeAccessory, doc, databasesMapPath);

                            hasParams = threeWayGlobeValve.GetParams();

                            if (hasParams == false)
                            {
                                doc.PostFailure(fm);
                                threeWayGlobeValve.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (threeWayGlobeValve.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParametersThreeWayGlobeValve(threeWayGlobeValve.DatabaseParameters, threeWayGlobeValve.PipeAccessory);
                            }

                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("test", ex.Message);
                }
            }

            foreach (ElementId id in data.GetModifiedElementIds())
            {
                if (!(doc.GetElement(id) is FamilyInstance pipeAccessory))
                    return;

                _pipeAccessoryName = pipeAccessory.Symbol.FamilyName;
                IEnumerable<ElementId> subElementTypeIds = pipeAccessory.GetSubComponentIds().Select(x => ((FamilyInstance)doc.GetElement(x)).GetTypeId());
                _familySubelementIds.AddRange(subElementTypeIds);

                if (_familySubelementIds.Contains(pipeAccessory.GetTypeId()))
                {
                    _familySubelementIds.Remove(pipeAccessory.GetTypeId());
                    continue;
                }

                if (_updatedElementids.Contains(pipeAccessory.Id))
                {
                    _updatedElementids.Remove(pipeAccessory.Id);
                    continue;
                }

                try
                {
                    bool? hasParams;

                    switch (_pipeAccessoryName)
                    {
                        case "COD_KOGELKRAAN":

                            StraightValve straightValve = new StraightValve(pipeAccessory, doc, databasesMapPath);

                            hasParams = straightValve.GetParams();

                            if (hasParams == false)
                            {
                                doc.PostFailure(fm);
                                straightValve.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (straightValve.ParametersAreTheSame())
                                    continue;

                                if (straightValve.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParamtersStraightValve(straightValve.DatabaseParameters, straightValve.PipeAccessory);

                            }

                            break;

                        case "COD_INREGELAFSLUITER":

                            BalanceValve balanceValve = new BalanceValve(pipeAccessory, doc, databasesMapPath);

                            hasParams = balanceValve.GetParams();

                            if (hasParams == false)
                            {
                                doc.PostFailure(fm);
                                balanceValve.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (balanceValve.ParametersAreTheSame())
                                    continue;

                                if (balanceValve.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParametersBalanceValve(balanceValve.DatabaseParameters, balanceValve.PipeAccessory);
                            }

                            break;

                        case "COD_VLINDERKLEP":

                            ButterflyValve butterflyValve = new ButterflyValve(pipeAccessory, doc, databasesMapPath);

                            hasParams = butterflyValve.GetParams();

                            if (hasParams == false)
                            {
                                doc.PostFailure(fm);
                                butterflyValve.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (butterflyValve.ParametersAreTheSame())
                                    continue;

                                if (butterflyValve.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParametersButterflyValve(butterflyValve.DatabaseParameters, butterflyValve.PipeAccessory);
                            }

                            break;

                        case "COD_Y_TYPEFILTER":

                            Strainer strainer = new Strainer(pipeAccessory, doc, databasesMapPath);

                            hasParams = strainer.GetParams();

                            if (hasParams == false)
                            {
                                doc.PostFailure(fm);
                                strainer.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (strainer.ParametersAreTheSame())
                                    continue;

                                if (strainer.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParametersStrainer(strainer.DatabaseParameters, strainer.PipeAccessory);
                            }

                            break;

                        case "COD_3WEGGLOBEVALVE":

                            ThreeWayGlobeValve threeWayGlobeValve = new ThreeWayGlobeValve(pipeAccessory, doc, databasesMapPath);

                            hasParams = threeWayGlobeValve.GetParams();

                            if (hasParams == false)
                            {
                                doc.PostFailure(fm);
                                threeWayGlobeValve.SetWrongValues();
                                continue;
                            }

                            if (hasParams == true)
                            {
                                if (threeWayGlobeValve.ParametersAreTheSame())
                                    continue;

                                if (threeWayGlobeValve.DatabaseParameters.Count == 0)
                                    throw new Exception("Geen parameters voor straightvalve!");

                                ElementSettings.SetCodraftParametersThreeWayGlobeValve(threeWayGlobeValve.DatabaseParameters, threeWayGlobeValve.PipeAccessory);
                            }

                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("test", ex.Message);
                }
            }
        }

        public string GetAdditionalInformation()
        {
            return "PipeAccessoryUpdater";
        }

        public ChangePriority GetChangePriority()
        {
            return ChangePriority.MEPAccessoriesFittingsSegmentsWires;
        }

        public UpdaterId GetUpdaterId()
        {
            return this.Id;
        }

        public string GetUpdaterName()
        {
            return "PipeAccessoryUpdater";
        }
    }
}
