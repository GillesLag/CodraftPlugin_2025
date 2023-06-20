using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodraftPlugin_Updaters;
using System;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CodraftPlugin_Library;

namespace CodraftPlugin_Loading
{
    public class LoadTabAndButtons : IExternalApplication
    {
        // Get the assembly name
        private string assemblyPath = Assembly.GetExecutingAssembly().Location;
        private const string globalParameterName = "RevitProjectMap";

        /// <summary>
        /// Creates a tabname, ribbonpanels and the buttons
        /// </summary>
        /// <param name="app"></param>
        public void AddRibbonPanel(UIControlledApplication app)
        {
            //Create Tab
            string tabName = "Codraft";
            app.CreateRibbonTab(tabName);

            //Create a ribbonpanels
            RibbonPanel updaters = app.CreateRibbonPanel(tabName, "Updaters");
            RibbonPanel tools = app.CreateRibbonPanel(tabName, "Tools");
            RibbonPanel Werkomgeving = app.CreateRibbonPanel(tabName, "Werkomgeving");

            //Create pipeupdater button
            PushButtonData pipeUpdaterData = new PushButtonData(
                "pipeUpdater",
                "Pipe Updater",
                assemblyPath,
                "CodraftPlugin_Loading.EnableDisablePipeUpdater");

            //add button to ribbon + tooltip and image
            PushButton pipeUpdaterButton = updaters.AddItem(pipeUpdaterData) as PushButton;
            pipeUpdaterButton.ToolTip = "Enables/Disables the pipeupdater.";
            pipeUpdaterButton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin_Loading;component/Resources/PipeUpdater.png"));

            //Create fittingupdater button
            PushButtonData fittingUpdaterData = new PushButtonData(
                "fittingUpdater",
                "Fitting Updater",
                assemblyPath,
                "CodraftPlugin_Loading.EnableDisableFittingUpdater");

            //add button to ribbon + tooltip and image
            PushButton fittingUpdaterButton = updaters.AddItem(fittingUpdaterData) as PushButton;
            fittingUpdaterButton.ToolTip = "Enables/Disables the fittingupdater.";
            fittingUpdaterButton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin_Loading;component/Resources/FittingUpdater.png"));

            //Create insulationupdater button
            PushButtonData insulationUpdaterData = new PushButtonData(
                "insulationUpdater",
                "Insulation Updater",
                assemblyPath,
                "CodraftPlugin_Loading.EnableDisableInsulationUpdater");

            //add button to ribbon + tooltip and image
            PushButton insulationUpdaterButton = updaters.AddItem(insulationUpdaterData) as PushButton;
            insulationUpdaterButton.ToolTip = "Enables/Disables the insulationupdater.";
            insulationUpdaterButton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin_Loading;component/Resources/InsulationUpdater.png"));

            //Create pipeAccessoryUpdater button
            PushButtonData pipeAccessoryUpdaterData = new PushButtonData(
                "pipeAccessoryUpdater",
                "Pipe Accessory\nUpdater",
                assemblyPath,
                "CodraftPlugin_Loading.EnableDisablePipeAccessoryUpdater");

            //add button to ribbon + tooltip and image
            PushButton pipeAccessoryUpdaterButton = updaters.AddItem(pipeAccessoryUpdaterData) as PushButton;
            pipeAccessoryUpdaterButton.ToolTip = "Enables/Disables the pipeAccessoryUpdater.";
            pipeAccessoryUpdaterButton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin_Loading;component/Resources/PipeAccessory.png"));

            //Create SelectFilterElements button
            PushButtonData selectFilterElementsData = new PushButtonData(
                "selectFilterElements",
                "Select\nFilter elements",
                assemblyPath,
                "CodraftPlugin_Loading.OpenFilterElementSelection");

            //add button to ribbon + tooltip and image
            PushButton selectFilterElementsButton = tools.AddItem(selectFilterElementsData) as PushButton;
            selectFilterElementsButton.ToolTip = "Select all elements based on filter";
            selectFilterElementsButton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin_Loading;component/Resources/Selection.png"));

            //Create Opstart Werkomgeving button
            PushButtonData opstartWerkomgevingData = new PushButtonData(
                "opstartWerkomgeving",
                "Start op\nWerkomgeving",
                assemblyPath,
                "CodraftPlugin_Loading.OpstartWerkomgeving");

            //add button to ribbon + tooltip and image
            PushButton opstartWerkomgevingButton = Werkomgeving.AddItem(opstartWerkomgevingData) as PushButton;
            opstartWerkomgevingButton.ToolTip = "Generate all pipetypes, materials, insulation, segments and system types.";
            opstartWerkomgevingButton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin_Loading;component/Resources/OpstartButton.png"));

            //Create Open Update All Insulation button
            PushButtonData updateAllInsulationData = new PushButtonData(
                "UpdateAllInsulation",
                "Update Isolatie",
                assemblyPath,
                "CodraftPlugin_Loading.OpenUpdateAllInsulation");

            //add button to ribbon + tooltip and image
            PushButton updateAllInsulationButton = tools.AddItem(updateAllInsulationData) as PushButton;
            updateAllInsulationButton.ToolTip = "Updates all pipe insulation based on the database.";
            updateAllInsulationButton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin_Loading;component/Resources/updateIcon.png"));

            //Create ChangeTagColor button
            PushButtonData changeTagColorData = new PushButtonData(
                "changeTagColor",
                "Change\nTag Color",
                assemblyPath,
                "CodraftPlugin_Loading.ChangeTagColor");

            //add button to ribbon + tooltip and image
            PushButton changeTagColorButton = tools.AddItem(changeTagColorData) as PushButton;
            changeTagColorButton.ToolTip = "Gives the tags the color of the systemtypes";
            changeTagColorButton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin_Loading;component/Resources/ChangeTagColor.png"));

            //Create HoldCloud button
            PushButtonData holdCloudData = new PushButtonData(
                "holdCloud",
                "Transform\nTo Hold Cloud",
                assemblyPath,
                "CodraftPlugin_Loading.HoldClouds");

            //add button to ribbon + tooltip and image
            PushButton holdCloudbutton = tools.AddItem(holdCloudData) as PushButton;
            holdCloudbutton.ToolTip = "Changes the hold clouds";
            holdCloudbutton.LargeImage = new BitmapImage(new Uri("pack://application:,,,/CodraftPlugin_Loading;component/Resources/holdCloud.png"));
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            //Unregister all updaters
            try
            {
                UpdaterManager.UnregisterPipeUpdater(application.ActiveAddInId);
                UpdaterManager.UnregisterFittingUpdater(application.ActiveAddInId);
                UpdaterManager.UnregisterInsulationUpdater(application.ActiveAddInId);
                UpdaterManager.UnregisterPipeAccessoryUpdater(application.ActiveAddInId);

                return Result.Succeeded;
            }

            catch (Exception)
            {
                return Result.Failed;
            }
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //Register all updaters
            try
            {
                AddRibbonPanel(application);
                UpdaterManager.RegisterPipeUpdater(application.ActiveAddInId);
                UpdaterManager.RegisterFittingUpdater(application.ActiveAddInId);
                UpdaterManager.RegisterInsulationUpdater(application.ActiveAddInId);
                UpdaterManager.RegisterPipeAccessoryUpdater(application.ActiveAddInId);

                UpdaterManager.SetExecutionOrder(application.ActiveAddInId);

                // Create failure definition Ids
                Guid guid1 = new Guid("45DFD462-806E-4B43-AA78-657851A2A38B");
                Guid guid2 = new Guid("4B81D4C5-185C-4830-8ECF-67370ADB06B0");

                FailureDefinitionId warningFittings = new FailureDefinitionId(guid1);
                FailureDefinitionId warningAccesories = new FailureDefinitionId(guid2);

                // Create failure definitions and add resolutions
                FailureDefinition fittingBestaatNiet = FailureDefinition.CreateFailureDefinition(warningFittings, FailureSeverity.Warning, "Fitting bestaat niet!");
                FailureDefinition AccessoryBestaatNiet = FailureDefinition.CreateFailureDefinition(warningAccesories, FailureSeverity.Warning, "PipeAccessory bestaat niet!");

                application.ControlledApplication.DocumentOpened += ControlledApplication_DocumentOpened;
                application.ControlledApplication.DocumentCreated += ControlledApplication_DocumentCreated;
                application.ControlledApplication.DocumentSaving += ControlledApplication_DocumentSaving;
                application.ControlledApplication.DocumentSavedAs += ControlledApplication_DocumentSavedAs;
                return Result.Succeeded;
            }

            catch (Exception)
            {

                return Result.Failed;
            }
        }

        private void ControlledApplication_DocumentSaving(object sender, Autodesk.Revit.DB.Events.DocumentSavingEventArgs e)
        {
            Document doc = e.Document;
            Transaction t = new Transaction(doc, "Set global parameter");
            t.Start();

            GlobalParameters.SetGlobalParameter(doc, globalParameterName);

            t.Commit();
        }

        private void ControlledApplication_DocumentCreated(object sender, Autodesk.Revit.DB.Events.DocumentCreatedEventArgs e)
        {
            if (!e.Document.IsFamilyDocument)
                TaskDialog.Show("Nieuw Project", "Slaag het project eerst op vooraleer je verder gaat.");
        }

        private void ControlledApplication_DocumentSavedAs(object sender, Autodesk.Revit.DB.Events.DocumentSavedAsEventArgs e)
        {
            Document doc = e.Document;
            Transaction t = new Transaction(doc, "Set global parameter");
            t.Start();

            GlobalParameters.SetGlobalParameter(doc, globalParameterName);

            t.Commit();
        }

        private void ControlledApplication_DocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs e)
        {
            Document doc = e.Document;
            Transaction t = new Transaction(doc, "Set global parameter");
            t.Start();

            GlobalParameters.SetGlobalParameter(doc, globalParameterName);

            t.Commit();
        }
    }
}
