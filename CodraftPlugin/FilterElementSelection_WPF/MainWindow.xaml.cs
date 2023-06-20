using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace FilterElementSelection_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly UIDocument uiDoc;
        readonly Document doc;
        List<string> filters = new List<string>();
        List<string> searchFilters = new List<string>();

        public MainWindow(UIDocument uiDoc)
        {
            InitializeComponent();

            this.uiDoc = uiDoc;
            doc = uiDoc.Document;

            // Get all filters in the current revit document.
            GetAllFilters();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Fill the listbox with all the filters names.
            lbFilters.ItemsSource = filters;
        }

        // Filters the list based on the users input
        private void tbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            filters.Clear();

            if (string.IsNullOrWhiteSpace(tbxSearch.Text))
                filters.AddRange(searchFilters);

            else
                filters.AddRange(searchFilters.Where(x => x.ToLower().Contains(tbxSearch.Text.ToLower())));

            lbFilters.Items.Refresh();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filterName = lbFilters.SelectedItem.ToString();

                // Select all elements in the document or view based on the filter that the user selected.
                SelectFilterElements(filterName);
                this.Close();
            }

            catch (NullReferenceException)
            {
                TaskDialog.Show("FoutMelding", "Selecteer een filter.");
            }

            catch (Exception)
            {
                TaskDialog.Show("FoutMelding", "Geen rules gespecifiëerd in de filter.");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Opens the filters menu in revit.
        private void btnFilters_Click(object sender, RoutedEventArgs e)
        {
            UIApplication uiApp = new UIApplication(uiDoc.Application.Application);
            RevitCommandId commandId = RevitCommandId.LookupPostableCommandId(PostableCommand.Filters);
            if (uiApp.CanPostCommand(commandId))
                uiApp.PostCommand(commandId);
        }

        // Refreshes the listbox
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetAllFilters();
            tbxSearch.Text = "";
        }

        private void SelectFilterElements(string filterName)
        {
            // Get the selected filter.
            ParameterFilterElement pfe = new FilteredElementCollector(doc)
                .OfClass(typeof(ParameterFilterElement))
                .Cast<ParameterFilterElement>()
                .First(x => x.Name == filterName);

            List<BuiltInCategory> bic = new List<BuiltInCategory>();

            // get all categories that the filter uses and put them in a list.
            foreach (ElementId i in pfe.GetCategories())
            {
                BuiltInCategory builtInCategory = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), i.ToString());
                if (builtInCategory == BuiltInCategory.OST_Sections)
                    bic.Add(BuiltInCategory.OST_Viewers);

                else
                    bic.Add(builtInCategory);
            }

            // Create a category filter for selection
            ElementMulticategoryFilter emf = new ElementMulticategoryFilter(bic);
            ICollection<ElementId> filteredElements;


            var test = pfe.GetElementFilter();

            // Search in whole documents
            if (rbSelectWhole.IsChecked == true)
            {
                filteredElements = new FilteredElementCollector(doc)
                    .WherePasses(emf)
                    .WherePasses(pfe.GetElementFilter())
                    .WhereElementIsNotElementType()
                    .ToElementIds();
            }

            // Search in active view
            else
            {
                filteredElements = new FilteredElementCollector(doc, uiDoc.ActiveView.Id)
                    .WherePasses(emf)
                    .WherePasses(pfe.GetElementFilter())
                    .WhereElementIsNotElementType()
                    .ToElementIds();
            }

            // Select filtered elements
            uiDoc.Selection.SetElementIds(filteredElements);
        }

        // Retrieve all filters in the document.
        private void GetAllFilters()
        {
            filters.Clear();
            searchFilters.Clear();

            IEnumerable<string> docFilters = new FilteredElementCollector(doc)
            .OfClass(typeof(ParameterFilterElement))
            .Select(x => x.Name);

            filters.AddRange(docFilters);
            searchFilters.AddRange(docFilters);

            lbFilters.Items.Refresh();
        }
    }
}
