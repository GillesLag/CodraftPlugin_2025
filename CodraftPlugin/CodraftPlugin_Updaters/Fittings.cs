using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodraftPlugin_DAL;
using CodraftPlugin_Exceptions;
using CodraftPlugin_Library;
using CodraftPlugin_Updaters.FittingTypes;
using System;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;

namespace CodraftPlugin_Updaters
{
    public class Fittings : IUpdater
    {
        private Guid _guid = new Guid("5176B710-F690-411B-A255-0C48FCEAF7FF");
        public UpdaterId Id { get; set; }
        private List<ElementId> fittingIds = new List<ElementId>();
        private Elbow elbow;
        private Tee tee;
        private Transition transition;
        private Tap tap;
        private Guid failureGuidPipeFittings = new Guid("45DFD462-806E-4B43-AA78-657851A2A38B");
        private string globalParameterName = "RevitProjectMap";

        private readonly string[] fittingTypes =
        {
            "Elbow",
            "Tee",
            "Transition_Concentrisch",
            "Transition_Excentrisch",
            "Tap"
        };

        public Fittings(AddInId addinId)
        {
            this.Id = new UpdaterId(addinId, _guid);
        }
        public void Execute(UpdaterData data)
        {
            // Get all the basic info from the document
            Document doc = data.GetDocument();

            string projectMapPath;
            string databasesMapPath;
            string insulationDatabasePath;
            string textFilesMapPath;

            // Check globalparater for projectfoldermap
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
            insulationDatabasePath = databasesMapPath + @"Isolatie.accdb";
            textFilesMapPath = projectMapPath + @"\RevitTextFiles\";

            FailureDefinitionId warning = new FailureDefinitionId(failureGuidPipeFittings);
            FailureMessage fm = new FailureMessage(warning);

            //
            // Update added fittings.
            //
            foreach (ElementId elemId in data.GetAddedElementIds())
            {
                // Get fitting from document.
                FamilyInstance fitting = (FamilyInstance)doc.GetElement(elemId);

                // check if fitting is a valid COD-family.
                if (!fittingTypes.Contains(fitting.Name)) continue;

                fittingIds.Add(elemId);

                // Handles the correct fittingType.
                try
                {
                    switch (fitting.Name)
                    {
                        case "Elbow":

                            // Create elbow object
                            elbow = new Elbow(fitting, doc, databasesMapPath, textFilesMapPath);

                            if (elbow.SystemType == "Undefined")
                                continue;

                            // Gets the params for the elbow
                            List<object> elbowParams = elbow.GetParamsFromDB();

                            if (elbowParams != null)
                                // set the parameters for the elbow in the document
                                ElementSettings.SetCodraftParametersElbow(elbowParams, fitting);

                            // check if the insulation updater is enabled
                            if (!UpdaterRegistry.IsUpdaterEnabled(new Insulation(doc.Application.ActiveAddInId).Id))
                                continue;

                            // create query for the database
                            string elbowNd = int.Parse(elbow.Nd1) > int.Parse(elbow.Nd2) ? elbow.Nd1 : elbow.Nd2;
                            string elbowQuery = $"SELECT *" +
                                $" FROM IsolatieTabel" +
                                $" WHERE Medium = \"{elbow.SystemType}\"" +
                                $" AND Nominale_diameter = {elbowNd};";

                            // get the insulation type and thickness
                            List<object> elbowLookupValues = FileOperations.LookupInsulation(elbowQuery, insulationDatabasePath);

                            if (elbowLookupValues == null)
                                continue;

                            // apply the insulation
                            ElementSettings.ApplyInsulation(elbow.Id, elbowLookupValues, doc);

                            break;

                        case "Tee":

                            tee = new Tee(fitting, doc, databasesMapPath, textFilesMapPath);

                            if (tee.SystemType == "Undefined")
                                continue;

                            List<object> teeParams = tee.GetParamsFromDB();

                            if (teeParams != null)
                                ElementSettings.SetCodraftParametersTee(teeParams, fitting);

                            break;

                        case "Transition_Concentrisch":
                        case "Transition_Excentrisch":

                            transition = new Transition(fitting, doc, databasesMapPath, textFilesMapPath);

                            if (transition.SystemType == "Undefined")
                                continue;

                            List<object> transParams = transition.GetParamsFromDB();

                            if (transParams != null)
                                ElementSettings.SetCodraftParametersTransition(transParams, fitting, transition.switchNds, transition.Excentrisch);

                            if (!UpdaterRegistry.IsUpdaterEnabled(new Insulation(doc.Application.ActiveAddInId).Id))
                                continue;

                            string transNd = int.Parse(transition.Nd1) > int.Parse(transition.Nd2) ? transition.Nd1 : transition.Nd2;
                            string transQuery = $"SELECT *" +
                                $" FROM IsolatieTabel" +
                                $" WHERE Medium = \"{transition.SystemType}\"" +
                                $" AND Nominale_diameter = {transNd};";

                            List<object> transLookupValues = FileOperations.LookupInsulation(transQuery, insulationDatabasePath);

                            if (transLookupValues == null)
                                continue;

                            ElementSettings.ApplyInsulation(transition.Id, transLookupValues, doc);

                            break;

                        case "Tap":

                            tap = new Tap(fitting, doc, databasesMapPath, textFilesMapPath);

                            if (tap.SystemType == "Undefined")
                                continue;

                            List<object> tapParams = tap.GetParamsFromDB();

                            if (tapParams != null)
                                ElementSettings.SetCodraftParametersTap(tapParams, fitting);

                            break;
                    }
                }

                catch (FittingDoesNotExistException)
                {
                    // Post warning
                    doc.PostFailure(fm);

                    switch (fitting.Name)
                    {
                        case "Elbow":
                            // set parameters for a wrong fitting.
                            ElementSettings.ElbowDoesNotExist(fitting);

                            break;

                        case "Tee":

                            // set parameters for a wrong fitting.
                            ElementSettings.TeeDoesNotExist(fitting);
                            break;

                        case "Transition_Excentrisch":
                        case "Transition_Concentrisch":

                            // set parameter for a wrong fitting
                            ElementSettings.TransitionDoesNotExist(fitting);
                            break;

                        case "Tap":

                            // set parameter for a wrong fitting
                            ElementSettings.TapDoesNotExist(fitting);
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error Fittings added element error", ex.Message);
                }
            }

            //
            // Update modified fittings.
            //
            foreach (ElementId elemId in data.GetModifiedElementIds())
            {
                // Get fitting from document.
                FamilyInstance fitting = (FamilyInstance)doc.GetElement(elemId);

                // check if fitting is a valid COD-family.
                if (!fittingTypes.Contains(fitting.Name)) continue;
                // check if fitting was an added element
                if (fittingIds.Contains(elemId))
                {
                    fittingIds.Remove(elemId);
                    continue;
                }

                fittingIds.Add(elemId);

                // check if fitting is gebruiker_gedefinieerd.
                if (fitting.LookupParameter("Gebruiker_gedefinieerd").AsInteger() == 1 && fitting.LookupParameter("Do_not_modify_gebruiker_gedefinieerd").AsInteger() == 1)
                    continue;

                else if (fitting.LookupParameter("Gebruiker_gedefinieerd").AsInteger() == 1)
                {
                    fitting.LookupParameter("COD_Fabrikant").Set("");
                    fitting.LookupParameter("COD_Type").Set("");
                    fitting.LookupParameter("COD_Materiaal").Set("");
                    fitting.LookupParameter("COD_Productcode").Set("");
                    fitting.LookupParameter("COD_Omschrijving").Set("");
                    fitting.LookupParameter("COD_Beschikbaar").Set("");
                    fitting.LookupParameter("Do_not_modify_gebruiker_gedefinieerd").Set(1);
                }
                    

                // Handles the correct fittingType.
                try
                {
                    switch (fitting.Name)
                    {
                        case "Elbow":

                            elbow = new Elbow(fitting, doc, databasesMapPath, textFilesMapPath);

                            if (elbow.SystemType == "Undefined")
                                continue;

                            List<object> parameters = elbow.GetParamsFromDB();

                            if (parameters == null || elbow.AreParamsTheSame(parameters))
                                continue;

                            ElementSettings.SetCodraftParametersElbow(parameters, fitting);

                            if (!UpdaterRegistry.IsUpdaterEnabled(new Insulation(doc.Application.ActiveAddInId).Id) || fitting.LookupParameter("COD_Isolatie").AsInteger() == 0)
                                continue;

                            string elbowNd = int.Parse(elbow.Nd1) > int.Parse(elbow.Nd2) ? elbow.Nd1 : elbow.Nd2;
                            string elbowQuery = $"SELECT *" +
                                $" FROM IsolatieTabel" +
                                $" WHERE Medium = \"{elbow.SystemType}\"" +
                                $" AND Nominale_diameter = {elbowNd};";

                            List<object> elbowLookupValues = FileOperations.LookupInsulation(elbowQuery, insulationDatabasePath);

                            if (elbowLookupValues == null)
                                continue;

                            if (fitting.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_TYPE).HasValue && ElementSettings.IsFittingInsulationApplied(fitting, elbowLookupValues))
                                continue;

                            ElementSettings.ApplyInsulation(elbow.Id, elbowLookupValues, doc);

                            break;

                        case "Tee":

                            tee = new Tee(fitting, doc, databasesMapPath, textFilesMapPath);

                            if (tee.SystemType == "Undefined")
                                continue;

                            List<object> teeParams = tee.GetParamsFromDB();

                            if (teeParams == null || tee.AreParamsTheSame(teeParams))
                                continue;

                            ElementSettings.SetCodraftParametersTee(teeParams, fitting);

                            break;

                        case "Transition_Concentrisch":
                        case "Transition_Excentrisch":

                            transition = new Transition(fitting, doc, databasesMapPath, textFilesMapPath);

                            if (transition.SystemType == "Undefined")
                                continue;

                            List<object> transitionParams = transition.GetParamsFromDB();

                            if (transitionParams == null || transition.AreParamsTheSame(transitionParams, transition.switchNds))
                                continue;

                            ElementSettings.SetCodraftParametersTransition(transitionParams, fitting, transition.switchNds, transition.Excentrisch);

                            if (!UpdaterRegistry.IsUpdaterEnabled(new Insulation(doc.Application.ActiveAddInId).Id) || fitting.LookupParameter("COD_Isolatie").AsInteger() == 0)
                                continue;

                            string transNd = int.Parse(transition.Nd1) > int.Parse(transition.Nd2) ? transition.Nd1 : transition.Nd2;
                            string transQuery = $"SELECT *" +
                                $" FROM IsolatieTabel" +
                                $" WHERE Medium = \"{elbow.SystemType}\"" +
                                $" AND Nominale_diameter = {transNd};";

                            List<object> transLookupValues = FileOperations.LookupInsulation(transQuery, insulationDatabasePath);

                            if (transLookupValues == null)
                                continue;

                            if (fitting.get_Parameter(BuiltInParameter.RBS_REFERENCE_INSULATION_TYPE).HasValue && ElementSettings.IsFittingInsulationApplied(fitting, transLookupValues))
                                continue;

                            ElementSettings.ApplyInsulation(elbow.Id, transLookupValues, doc);

                            break;

                        case "Tap":

                            tap = new Tap(fitting, doc, databasesMapPath, textFilesMapPath);

                            if (tap.SystemType == "Undefined")
                                continue;

                            List<object> tapParams = tap.GetParamsFromDB();

                            if (tapParams == null || tap.AreParamsTheSame(tapParams))
                                continue;

                            ElementSettings.SetCodraftParametersTap(tapParams, fitting);

                            break;
                    }
                }

                catch (FittingDoesNotExistException)
                {
                    switch (fitting.Name)
                    {
                        case "Elbow":
                            // Checks if the fitting is already wrong.
                            if (elbow.IsAlreadyWrong()) continue;

                            // Post warning
                            doc.PostFailure(fm);

                            // set parameters for wrong fitting.
                            ElementSettings.ElbowDoesNotExist(fitting);

                            break;

                        case "Tee":
                            if (tee.IsAlreadyWrong()) continue;

                            // Post warning
                            doc.PostFailure(fm);

                            ElementSettings.TeeDoesNotExist(fitting);

                            break;

                        case "Transition_Excentrisch":
                        case "Transition_Concentrisch":
                            if (transition.IsAlreadyWrong()) continue;

                            // Post warning
                            doc.PostFailure(fm);

                            ElementSettings.TransitionDoesNotExist(fitting);

                            break;

                        case "Tap":
                            if (tap.IsAlreadyWrong()) continue;

                            // Post warning
                            doc.PostFailure(fm); ;

                            // set parameter for a wrong fitting
                            ElementSettings.TapDoesNotExist(fitting);
                            break;

                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Fittings modified elements", ex.Message);
                }
            }
        }

        public string GetAdditionalInformation()
        {
            return "FitingUpdater";
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
            return "FittingUpdater";
        }
    }
}
