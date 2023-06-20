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
using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using CodraftPlugin_DAL;

namespace CodraftPlugin_UpdateAllInsulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const float feetToMm = 304.8f;

        private Document doc;
        private List<PipingSystemType> systemTypes = new List<PipingSystemType>();
        private List<PipingSystemType> searchSystemTypes = new List<PipingSystemType>();
        private List<PipingSystemType> allSystemTypes = new List<PipingSystemType>();
        private List<PipingSystemType> systemTypesToUpdate = new List<PipingSystemType>();

        private string connectionString = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source =";
        private string SQLstring;
        public MainWindow(Document doc)
        {
            this.doc = doc;
            InitializeComponent();
        }

        private void UpdateInsulation_Loaded(object sender, RoutedEventArgs e)
        {
            string root = doc.PathName;
            int index = root.LastIndexOf('\\') + 1;
            string pathDatabase = root.Substring(0, index) + @"\RevitDatabases\Isolatie.accdb";
            connectionString += pathDatabase;

            GetAllSystemTypes(this.doc);

            allSystemTypes.AddRange(systemTypes.OrderBy(x => x.Name));

            lbAllMediums.ItemsSource = allSystemTypes;
            lbMediumsToUpdate.ItemsSource = systemTypesToUpdate;

            lbAllMediums.DisplayMemberPath = "Name";
            lbMediumsToUpdate.DisplayMemberPath = "Name";
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lbMediumsToUpdate.Items.IsEmpty)
            {
                MessageBox.Show("Select SystemTypes to update!", "Error");
                return;
            }

            //Get all the data for the  mediums that are changed from the database.
            SetSqlString();

            try
            {
                //Retrieves the data from the database in a tree data structure.
                var isolatieData = FileOperations.IsolatieData(SQLstring, connectionString);

                Transaction t = new Transaction(doc, "Update Pipeinsulation");

                t.Start();

                foreach (var medium in isolatieData)
                {
                    IEnumerable<Pipe> pipes = new FilteredElementCollector(doc)
                        .OfClass(typeof(Pipe))
                        .Cast<Pipe>()
                        .Where(x => x.MEPSystem != null && doc.GetElement(x.MEPSystem.GetTypeId()).Name == medium.Key);

                    if (!pipes.Any() || pipes == null)
                        continue;

                    foreach (var isol in medium.Value)
                    {
                        ElementId isolId;
                        try
                        {
                            isolId = new FilteredElementCollector(doc)
                                            .OfClass(typeof(PipeInsulationType))
                                            .First(x => x.Name == isol.Key).Id;
                        }
                        catch (Exception)
                        {
                            continue;
                        }

                        foreach (var isolDikte in isol.Value)
                        {
                            List<double> dn = isolDikte.Value;

                            IEnumerable<Pipe> p = pipes.Where(x => dn.Contains(double.Parse(x.get_Parameter(BuiltInParameter.RBS_CALCULATED_SIZE).AsString().Split(' ').First())));

                            if (!p.Any())
                                continue;

                            IEnumerable<FamilyInstance> fittings = new FilteredElementCollector(doc)
                                .OfCategory(BuiltInCategory.OST_PipeFitting)
                                .WhereElementIsNotElementType()
                                .Cast<FamilyInstance>()
                                .Where(x => x.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString() == medium.Key)
                                .Where(x => dn.Contains(x.get_Parameter(BuiltInParameter.RBS_CALCULATED_SIZE).AsString().Split('-')
                                    .Select(y => double.Parse(y.Split(' ').First()))
                                        .Max()));

                            int loopPipes = p.Count();
                            int loopFittings = fittings.Count();

                            for (int i = 0; i < loopPipes; i++)
                            {
                                Pipe pipe = p.ElementAt(i);

                                doc.Delete(InsulationLiningBase.GetInsulationIds(doc, pipe.Id));

                                PipeInsulation.Create(doc, pipe.Id, isolId, isolDikte.Key / feetToMm);

                                pipe.LookupParameter("COD_Isolatie").Set(1);
                                pipe.LookupParameter("Do_not_modify_COD_Isolatie").Set(1);
                                pipe.LookupParameter("Do_not_modify_Isolatie_dikte").Set(isolDikte.Key);
                                pipe.LookupParameter("Do_not_modify_Isolatie_type").Set(isol.Key);
                                pipe.LookupParameter("Gebruiker_gedefinieerd").Set(0);
                                pipe.LookupParameter("Do_not_modify_gebruiker_gedefinieerd").Set(0);

                            }

                            for (int i = 0; i < loopFittings; i++)
                            {
                                FamilyInstance fitting = fittings.ElementAt(i);

                                doc.Delete(InsulationLiningBase.GetInsulationIds(doc, fitting.Id));

                                PipeInsulation.Create(doc, fitting.Id, isolId, isolDikte.Key / feetToMm);
                            }
                        }
                    }
                }

                t.Commit();
            }

            catch (Exception i)
            {
                MessageBox.Show(i.Message, "Error");
            }

            MessageBox.Show("All pipe insulation is updateted!", "Update Insulation");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnTransferRight_Click(object sender, RoutedEventArgs e)
        {
            if (lbAllMediums.SelectedItems != null)
                Transfer(lbAllMediums);
        }

        private void btnTransferAllRight_Click(object sender, RoutedEventArgs e)
        {
            TransferAll();
        }

        private void btnTransferLeft_Click(object sender, RoutedEventArgs e)
        {
            if (lbMediumsToUpdate.SelectedItems != null)
                Transfer(lbMediumsToUpdate);
        }

        private void btnTransferAllLeft_Click(object sender, RoutedEventArgs e)
        {
            TransferAll();
        }

        private void GetAllSystemTypes(Document doc)
        {
            List<PipingSystemType> sTypes = new FilteredElementCollector(doc)
                .OfClass(typeof(PipingSystemType))
                .Cast<PipingSystemType>()
                .ToList();

            systemTypes = new List<PipingSystemType>(sTypes);
            searchSystemTypes = new List<PipingSystemType>(sTypes);
        }

        private void Transfer(ListBox listBox)
        {
            try
            {
                if (listBox == lbAllMediums)
                {
                    foreach (PipingSystemType i in listBox.SelectedItems)
                    {
                        if (!systemTypesToUpdate.Contains(i))
                            systemTypesToUpdate.Add(i);
                    }
                }

                else
                {
                    foreach (PipingSystemType i in listBox.SelectedItems)
                    {
                        if (!allSystemTypes.Contains(i))
                            systemTypesToUpdate.Remove(i);
                    }
                }

                allSystemTypes.Clear();
                searchSystemTypes.Clear();
                allSystemTypes.AddRange(systemTypes.
                    Where(x => !systemTypesToUpdate.Contains(x))
                    .OrderBy(x => x.Name));
                searchSystemTypes.AddRange(systemTypes.
                    Where(x => !systemTypesToUpdate.Contains(x))
                    .OrderBy(x => x.Name));

                lbAllMediums.Items.Refresh();
                lbMediumsToUpdate.Items.Refresh();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "error");
            }

            SearchMediums();
        }

        private void TransferAll()
        {
            try
            {
                if (btnTransferAllRight.IsMouseOver)
                    systemTypesToUpdate.AddRange(allSystemTypes);

                else
                    systemTypesToUpdate.Clear();

                allSystemTypes.Clear();
                searchSystemTypes.Clear();
                allSystemTypes.AddRange(systemTypes.
                    Where(x => !systemTypesToUpdate.Contains(x))
                    .OrderBy(x => x.Name));
                searchSystemTypes.AddRange(systemTypes.
                    Where(x => !systemTypesToUpdate.Contains(x))
                    .OrderBy(x => x.Name));

                lbAllMediums.Items.Refresh();
                lbMediumsToUpdate.Items.Refresh();
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
            }
            SearchMediums();
        }

        private void SetSqlString()
        {
            SQLstring = "SELECT IsolatieTabel.Medium, IsolatieTabel.Isolatie_materiaal, IsolatieTabel.Isolatie_dikte, IsolatieTabel.Nominale_diameter"
             + " FROM IsolatieTabel"
             + " WHERE(((IsolatieTabel.Medium) = ";

            for (int i = 0; i < lbMediumsToUpdate.Items.Count; i++)
            {
                if (i == 0)
                {
                    SQLstring += $"\"{((MEPSystemType)lbMediumsToUpdate.Items[i]).Name}\"";
                }

                else
                    SQLstring += $" OR (IsolatieTabel.Medium)= \"{((MEPSystemType)lbMediumsToUpdate.Items[i]).Name}\"";
            }

            SQLstring += "));";
        }

        private void tbxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchMediums();
        }

        private void SearchMediums()
        {
            allSystemTypes.Clear();

            if (string.IsNullOrWhiteSpace(tbxSearch.Text))
                allSystemTypes.AddRange(searchSystemTypes);

            else
                allSystemTypes.AddRange(searchSystemTypes.Where(x => x.Name.ToLower().Contains(tbxSearch.Text.ToLower())));

            lbAllMediums.Items.Refresh();
        }
    }
}
