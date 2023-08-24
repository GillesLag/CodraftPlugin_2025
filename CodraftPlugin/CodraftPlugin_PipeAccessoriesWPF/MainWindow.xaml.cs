using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CodraftPlugin_PipeAccessoriesWPF.Model;
using CodraftPlugin_Library;
using CodraftPlugin_DAL;
using Newtonsoft.Json.Linq;

namespace CodraftPlugin_PipeAccessoriesWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double feetToMm = 304.8;

        private string _accessoryName;
        private string _connectionString;
        private string _sqlQuery;
        private string _databaseFilePath;
        private List<string> _callingParams;
        private FamilyInstance _pipeAccessory;
        private List<BaseAccessory> _accessories = new List<BaseAccessory>();
        private JObject _parameterConfiguration;

        public bool hasChosenAccessory { get; private set; } = false;
        public MainWindow(FamilyInstance PipeAccessory, string name, string connectionString, string sqlQuery, string databaseFilePath, List<string> callingParams, JObject file)
        {
            InitializeComponent();

            _accessoryName = name;
            _connectionString = connectionString;
            _sqlQuery = sqlQuery;
            _pipeAccessory = PipeAccessory;
            _databaseFilePath = databaseFilePath;
            _callingParams = callingParams;
            _parameterConfiguration = file;

            FillDataGrid();
        }

        private void fdgCataloog_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyDescriptor propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
            e.Column.Header = propertyDescriptor.DisplayName;
            if (propertyDescriptor.DisplayName == "Accessory")
            {
                e.Cancel = true;
            }
        }

        private void FillDataGrid()
        {
            using (OleDbConnection connection = new OleDbConnection(_connectionString))
            {
                OleDbCommand command = new OleDbCommand(_sqlQuery, connection);
                connection.Open();

                BaseAccessory accessory = null;

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        switch (_accessoryName)
                        {
                            case "StraightValve":
                                accessory = new StraightValve(_pipeAccessory ,(string)reader["Manufacturer"], (string)reader["Type"], (string)reader["Material"],
                                    (string)reader["Product Code"], (string)reader["Omschrijving"], (string)reader["Beschikbaar"], (int)reader["D1"], (double)reader["Lengte"],
                                    (double)reader["Hendel_lengte"], (double)reader["Hendel_breedte"], (double)reader["Hendel_hoogte"], (double)reader["Motor_lengte"],
                                    (double)reader["Motor_breedte"], (double)reader["Motor_hoogte"], (double)reader["Wormwiel_straal"], (double)reader["Wormwiel_staaf_straal"],
                                    (double)reader["Operator_hoogte"], (double)reader["Vlinderhendel_diameter"], (int)reader["Uiteinde_1_type"], (int)reader["Uiteinde_2_type"],
                                    (double)reader["Uiteinde_1_maat"], (double)reader["Uiteinde_2_maat"], (double)reader["L1"], (double)reader["L2"], (double)reader["PipeOD"]);
                                break;

                            case "BalanceValve":
                                accessory = new BalanceValve(_pipeAccessory, (string)reader["Manufacturer"], (string)reader["Type"], (string)reader["Material"],
                                    (string)reader["Product Code"], (string)reader["Omschrijving"], (string)reader["Beschikbaar"], (int)reader["D1"], (double)reader["Lengte"],
                                    (double)reader["PipeOD"], (int)reader["Uiteinde_1_type"], (int)reader["Uiteinde_2_type"], (double)reader["Uiteinde_1_maat"], (double)reader["Uiteinde_2_maat"],
                                    (double)reader["L1"], (double)reader["L2"]);
                                break;

                            case "Strainer":
                                accessory = new Strainer(_pipeAccessory, (string)reader["Manufacturer"], (string)reader["Type"], (string)reader["Material"],
                                    (string)reader["Product Code"], (string)reader["Omschrijving"], (string)reader["Beschikbaar"], (int)reader["D1"],
                                    (double)reader["PipeOD"], (double)reader["Height"], (double)reader["CompLen"], (double)reader["BranchOffset"], (int)reader["Uiteinde_1_type"],
                                    (int)reader["Uiteinde_2_type"], (double)reader["L1"], (double)reader["L2"], (double)reader["Uiteinde_1_maat"], (double)reader["Uiteinde_2_maat"]);
                                break;

                            case "ThreeWayGlobeValve":
                                accessory = new ThreeWayGlobeValve(_pipeAccessory, (string)reader["Manufacturer"], (string)reader["Type"], (string)reader["Material"],
                                    (string)reader["Product Code"], (string)reader["Omschrijving"], (string)reader["Beschikbaar"], (int)reader["D1"],
                                    (double)reader["PipeOD1"], (double)reader["Lengte"], (double)reader["Lengte_3"], (int)reader["Uiteinde_1_type"], (int)reader["Uiteinde_2_type"],
                                    (int)reader["Uiteinde_3_type"], (double)reader["L1"], (double)reader["L2"], (double)reader["L3"], (double)reader["Uiteinde_1_maat"], (double)reader["Uiteinde_2_maat"],
                                    (double)reader["Uiteinde_3_maat"], (double)reader["a"], (double)reader["d"], (double)reader["e"], (double)reader["b"], (double)reader["c"], (double)reader["d"]);
                                break;

                            case "ButterflyValve":
                                accessory = new ButterflyValve(_pipeAccessory, (string)reader["Manufacturer"], (string)reader["Type"], (string)reader["Material"],
                                    (string)reader["Product Code"], (string)reader["Omschrijving"], (string)reader["Beschikbaar"], (int)reader["D1"], (double)reader["CompOD"], (double)reader["CompLen"],
                                    (int)reader["D1"], (double)reader["b"], (double)reader["b"], (double)reader["b"], (double)reader["c"], (double)reader["d"],
                                    (double)reader["Thickness"], (double)reader["BladeDiameter"], (double)reader["c"], (double)reader["d"]);
                                break;

                            default:
                                break;
                        }

                        _accessories.Add(accessory);
                    }
                }

                fdgCataloog.ItemsSource = _accessories;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOke_Click(object sender, RoutedEventArgs e)
        {
            BaseAccessory ba = fdgCataloog.SelectedItem as BaseAccessory;

            if (ba == null)
            {
                TaskDialog.Show("Fout", "Selecteer een accessory.");
                return;
            }

            List<object> paramList = new List<object>();

            switch (_accessoryName)
            {
                case "StraightValve":
                    StraightValve straightValve = (StraightValve)ba;
                    paramList.Add(Math.Round(straightValve.Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.HendelLengte / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.HendelBreedte / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.HendelHoogte / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.MotorLengte / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.MotorBreedte / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.MotorHoogte / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.WormwielStraal / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.WormwielStaafStraal / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.OperatorHoogte / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.VlinderhendelDiameter / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.BuitenDiameter / feetToMm, 4));
                    paramList.Add(straightValve.UiteindeType1);
                    paramList.Add(straightValve.UiteindeType2);
                    paramList.Add(Math.Round(straightValve.UiteindeMaat1 / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.UiteindeMaat2 / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.L1 / feetToMm, 4));
                    paramList.Add(Math.Round(straightValve.L2 / feetToMm, 4));
                    paramList.Add(straightValve.Fabrikant);
                    paramList.Add(straightValve.Type);
                    paramList.Add(straightValve.Materiaal);
                    paramList.Add(straightValve.ProductCode);
                    paramList.Add(straightValve.Omschrijving);
                    paramList.Add(straightValve.Beschikbaar);

                    ElementSettings.SetCodraftParamtersStraightValve(paramList, ba.Accessory, _parameterConfiguration);
                    break;

                case "BalanceValve":
                    BalanceValve balanceValve = (BalanceValve)ba;
                    paramList.Add(Math.Round(balanceValve.Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(balanceValve.BuitenDiameter / feetToMm, 4));
                    paramList.Add(balanceValve.UiteindeType1);
                    paramList.Add(balanceValve.UiteindeType2);
                    paramList.Add(Math.Round(balanceValve.UiteindeMaat1 / feetToMm, 4));
                    paramList.Add(Math.Round(balanceValve.UiteindeMaat2 / feetToMm, 4));
                    paramList.Add(Math.Round(balanceValve.L1 / feetToMm, 4));
                    paramList.Add(Math.Round(balanceValve.L2 / feetToMm, 4));
                    paramList.Add(balanceValve.Fabrikant);
                    paramList.Add(balanceValve.Type);
                    paramList.Add(balanceValve.Materiaal);
                    paramList.Add(balanceValve.ProductCode);
                    paramList.Add(balanceValve.Omschrijving);
                    paramList.Add(balanceValve.Beschikbaar);

                    ElementSettings.SetCodraftParametersBalanceValve(paramList, ba.Accessory, _parameterConfiguration);
                    break;

                case "Strainer":
                    Strainer strainer = (Strainer)ba;
                    paramList.Add(Math.Round(strainer.BuitenDiameter / feetToMm, 4));
                    paramList.Add(Math.Round(strainer.Hoogte / feetToMm, 4));
                    paramList.Add(Math.Round(strainer.Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(strainer.Offset / feetToMm, 4));
                    paramList.Add(strainer.UiteindeType1);
                    paramList.Add(strainer.UiteindeType2);
                    paramList.Add(Math.Round(strainer.L1 / feetToMm, 4));
                    paramList.Add(Math.Round(strainer.L2 / feetToMm, 4));
                    paramList.Add(Math.Round(strainer.UiteindeMaat1 / feetToMm, 4));
                    paramList.Add(Math.Round(strainer.UiteindeMaat2 / feetToMm, 4));
                    paramList.Add(strainer.Fabrikant);
                    paramList.Add(strainer.Type);
                    paramList.Add(strainer.Materiaal);
                    paramList.Add(strainer.ProductCode);
                    paramList.Add(strainer.Omschrijving);
                    paramList.Add(strainer.Beschikbaar);

                    ElementSettings.SetCodraftParametersStrainer(paramList, ba.Accessory, _parameterConfiguration);
                    break;

                case "ThreeWayGlobeValve":
                    ThreeWayGlobeValve threeWayGlobeValve = (ThreeWayGlobeValve)ba;
                    paramList.Add(Math.Round(threeWayGlobeValve.BuitenDiameter / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.Lengte3 / feetToMm, 4));
                    paramList.Add(threeWayGlobeValve.UiteindeType1);
                    paramList.Add(threeWayGlobeValve.UiteindeType2);
                    paramList.Add(threeWayGlobeValve.UiteindeType3);
                    paramList.Add(Math.Round(threeWayGlobeValve.L1 / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.L2 / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.L3 / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.UiteindeMaat1 / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.UiteindeMaat2 / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.UiteindeMaat3 / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.MotorLengte / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.MotorBreedte / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.MotorHoogte / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.HoogteOperator / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.WormwielDiameter / feetToMm, 4));
                    paramList.Add(Math.Round(threeWayGlobeValve.WormwielLengte / feetToMm, 4));
                    paramList.Add(threeWayGlobeValve.Fabrikant);
                    paramList.Add(threeWayGlobeValve.Type);
                    paramList.Add(threeWayGlobeValve.Materiaal);
                    paramList.Add(threeWayGlobeValve.ProductCode);
                    paramList.Add(threeWayGlobeValve.Omschrijving);
                    paramList.Add(threeWayGlobeValve.Beschikbaar);

                    ElementSettings.SetCodraftParametersThreeWayGlobeValve(paramList, ba.Accessory, _parameterConfiguration);
                    break;

                case "ButterflyValve":
                    ButterflyValve butterflyValve = (ButterflyValve)ba;
                    paramList.Add(Math.Round(butterflyValve.BuitenDiameterTotaal / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.BuitenDiameter / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.StaafLengte / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.HendelLengte / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.MotorLengte / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.MotorHoogte / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.MotorBreedte / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.BladeDikte / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.BladeDiameter / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.WormwielDiameter / feetToMm, 4));
                    paramList.Add(Math.Round(butterflyValve.WormwielLengte / feetToMm, 4));
                    paramList.Add(butterflyValve.Fabrikant);
                    paramList.Add(butterflyValve.Type);
                    paramList.Add(butterflyValve.Materiaal);
                    paramList.Add(butterflyValve.ProductCode);
                    paramList.Add(butterflyValve.Omschrijving);
                    paramList.Add(butterflyValve.Beschikbaar);

                    ElementSettings.SetCodraftParametersButterflyValve(paramList, ba.Accessory, _parameterConfiguration);
                    break;

                default:
                    break;
            }

            if (cbRememberChoice.IsChecked == true)
            {
                string revitFilePath = _pipeAccessory.Document.PathName;
                string rememberFilePath = revitFilePath.Substring(0, revitFilePath.LastIndexOf('\\') + 1) + "RevitTextFiles\\RememberMePipeAccessories.txt";
                FileOperations.RememberMe(paramList, rememberFilePath, _callingParams);
            }

            hasChosenAccessory = true;
            this.Close();
        }

        private void btnOpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\Program Files\Microsoft Office\root\Office16\MSACCESS.EXE");

            System.Threading.Thread.Sleep(1500);

            System.Diagnostics.Process.Start(_databaseFilePath);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _accessories.Clear();
            fdgCataloog.ItemsSource = null;
            FillDataGrid();
            fdgCataloog.Items.Refresh();
        }
    }
}
