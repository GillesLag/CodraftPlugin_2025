using Autodesk.Revit.DB;
using CodraftPlugin_DAL;
using CodraftPlugin_Library;
using CodraftPlugin_UIDatabaseWPF.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Input;
using Autodesk.Revit.UI;

namespace CodraftPlugin_UIDatabaseWPF.ModelView
{
    public class FittingModelView : INotifyPropertyChanged
    {
        const float feetToMm = 304.8f;
        #region Private Fields

        private string connectionString;
        private string query;
        private string databaseFilePath;
        private FamilyInstance fittingModel;
        private bool _RememberChoice;
        private bool _switchNd;
        private int _excentrisch;
        private string rememberMeFile;
        private List<string> parameters;
        private double maxDiameter;

        #endregion

        #region Properties

        public ObservableCollection<FittingModel> Fittings { get; set; } = new ObservableCollection<FittingModel>();
        public ICommand RefreshCommand => new RelayCommand(Refresh);
        public ICommand OpenDatabaseCommand => new RelayCommand(OpenDatabase);
        public Action CloseWindow { get; set; }
        public ICommand OkeCommand => new RelayCommand(Oke);
        public ICommand CloseCommand => new RelayCommand(CloseWindow);
        public bool RememberChoice
        {
            get { return _RememberChoice; }
            set
            {
                _RememberChoice = value;

                OnPropertyChanged(nameof(RememberChoice));
            }
        }

        #endregion

        #region Constructors

        public FittingModelView(string connectionString, string databaseFilePath, string rememberMeFilePath, string strSQL, FamilyInstance fitting,
            List<string> parameters, bool switchNd, int excentrisch, double maxDiameter = 0)
        {
            this.connectionString = connectionString;
            this.databaseFilePath = databaseFilePath;
            this.query = strSQL;
            this.fittingModel = fitting;
            this.rememberMeFile = rememberMeFilePath;
            this.parameters = parameters;
            this._switchNd = switchNd;
            this._excentrisch = excentrisch;
            this.maxDiameter = maxDiameter;

            FillList();
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Fill the datatable with information from the database
        /// </summary>
        public void FillList()
        {
            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    OleDbCommand command = new OleDbCommand(query, connection);
                    connection.Open();

                    try
                    {
                        FittingModel fitting = null;
                        var test = fittingModel.Name;

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                switch (fittingModel.Name)
                                {
                                    case "Elbow":
                                        fitting = new Elbow((string)reader["Fabrikant"], (string)reader["Type"], (string)reader["Materiaal"], (string)reader["ProductCode"],
                                            (string)reader["Beschikbaar"], (string)reader["Omschrijving"], fittingModel,
                                            (double)reader["Nominale_diameter_1"], (double)reader["Nominale_diameter_2"], (double)reader["Buitendiameter_1"], (double)reader["Buitendiameter_2"],
                                            (double)reader["Standaard_hoek"], (double)reader["Hoek_inkortbaar"], (double)reader["Uiteinde_1_type"], (double)reader["Uiteinde_2_Type"],
                                            (double)reader["Uiteinde_1_lengte"], (double)reader["Uiteinde_2_lengte"], (double)reader["Uiteinde_1_maat"], (double)reader["Uiteinde_2_maat"],
                                            (double)reader["Flens_dikte"], (double)reader["Center_straal"]);
                                        break;

                                    case "Tee":
                                        fitting = new Tee((string)reader["Fabrikant"], (string)reader["Type"], (string)reader["Materiaal"], (string)reader["ProductCode"],
                                           (string)reader["Beschikbaar"], (string)reader["Omschrijving"], fittingModel, (double)reader["Nominale_diameter_1"],
                                           (double)reader["Nominale_diameter_2"], (double)reader["Nominale_diameter_3"], (double)reader["Buitendiameter_1"], (double)reader["Buitendiameter_2"],
                                           (double)reader["Buitendiameter_3"], (double)reader["Standaard_hoek"], (double)reader["Uiteinde_1_type"], (double)reader["Uiteinde_2_Type"],
                                           (double)reader["Uiteinde_3_Type"], (double)reader["Uiteinde_1_lengte"], (double)reader["Uiteinde_2_lengte"], (double)reader["Uiteinde_3_lengte"],
                                           (double)reader["Uiteinde_1_maat"], (double)reader["Uiteinde_2_maat"], (double)reader["Uiteinde_3_maat"], (double)reader["Lengte"], (double)reader["Center_uiteinde_1"],
                                           (double)reader["Center_uiteinde_3"], (double)reader["Flens_dikte1"], (double)reader["Flens_dikte2"], (double)reader["Flens_dikte3"]);

                                        break;

                                    case "Transition_Concentrisch":
                                    case "Transition_Excentrisch":
                                        fitting = new Transition((string)reader["Fabrikant"], (string)reader["Type"], (string)reader["Materiaal"], (string)reader["ProductCode"],
                                           (string)reader["Beschikbaar"], (string)reader["Omschrijving"], fittingModel,
                                           (double)reader["Nominale_diameter_1"], (double)reader["Nominale_diameter_2"], (double)reader["Buitendiameter_1"], (double)reader["Buitendiameter_2"],
                                           (double)reader["Uiteinde_1_type"], (double)reader["Uiteinde_2_Type"], (double)reader["Uiteinde_1_lengte"], (double)reader["Uiteinde_2_lengte"],
                                           (double)reader["Uiteinde_1_maat"], (double)reader["Uiteinde_2_maat"], (double)reader["Flens_dikte_1"], (double)reader["Flens_dikte_2"], (double)reader["Lengte"]);

                                        break;

                                    case "Tap":
                                        fitting = new Tap((string)reader["Fabrikant"], (string)reader["Type"], (string)reader["Materiaal"], (string)reader["ProductCode"],
                                           (string)reader["Beschikbaar"], (string)reader["Omschrijving"], fittingModel, (double)reader["Nominale_diameter"], (double)reader["Buitendiameter"],
                                           (double)reader["Lengte"], maxDiameter);

                                        break;
                                }

                                Fittings.Add(fitting);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("FittingModelView reader error", ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("FittingModelView connection error", ex.Message);
            }

            OnPropertyChanged(nameof(Fittings));
        }

        /// <summary>
        /// gets the chosen fitting and inject the parameters int the fitting from the document.
        /// </summary>
        /// <param name="fitting"></param>
        public void Oke(FittingModel fitting)
        {
            if (fitting == null)
            {
                MessageBox.Show("Selecteer een fitting", "Geen fitting geselecteerd!");
                return;
            }

            List<object> paramList = new List<object>();

            switch (fitting.Fitting.Name)
            {
                case "Elbow":
                    Elbow elbow = (Elbow)fitting;
                    paramList.Add(Math.Round(elbow.Buitendiameter_1 / feetToMm, 4));
                    paramList.Add(Math.Round(elbow.Buitendiameter_2 / feetToMm, 4));
                    paramList.Add(Math.Round(elbow.CenterStraal / feetToMm, 4));
                    paramList.Add(elbow.Uiteinde_1_Type);
                    paramList.Add(elbow.Uiteinde_2_Type);
                    paramList.Add(Math.Round(elbow.Uiteinde_1_Maat / feetToMm, 4));
                    paramList.Add(Math.Round(elbow.Uiteinde_2_Maat / feetToMm, 4));
                    paramList.Add(Math.Round(elbow.Uiteinde_1_Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(elbow.Uiteinde_2_Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(elbow.FlensDikte / feetToMm, 4));
                    paramList.Add(elbow.Hoek);
                    paramList.Add(elbow.Fabrikant);
                    paramList.Add(elbow.Type);
                    paramList.Add(elbow.Materiaal);
                    paramList.Add(elbow.ProductCode);
                    paramList.Add(elbow.Omschrijving);
                    paramList.Add(elbow.Beschikbaar);

                    ElementSettings.SetCodraftParametersElbow(paramList, this.fittingModel);
                    break;

                case "Tee":
                    Tee tee = (Tee)fitting;
                    paramList.Add(Math.Round(tee.Buitendiameter_1 / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Buitendiameter_2 / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Buitendiameter_3 / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Center_Uiteinde_3 / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Center_Uiteinde_1 / feetToMm, 4));
                    paramList.Add(tee.Uiteinde_1_Type);
                    paramList.Add(tee.Uiteinde_2_Type);
                    paramList.Add(tee.Uiteinde_3_Type);
                    paramList.Add(Math.Round(tee.Uiteinde_1_Maat / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Uiteinde_2_Maat / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Uiteinde_3_Maat / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Uiteinde_1_Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Uiteinde_2_Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(tee.Uiteinde_3_Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(tee.FlensDikte1 / feetToMm, 4));
                    paramList.Add(Math.Round(tee.FlensDikte2 / feetToMm, 4));
                    paramList.Add(Math.Round(tee.FlensDikte3 / feetToMm, 4));
                    paramList.Add(tee.Hoek);
                    paramList.Add(tee.Fabrikant);
                    paramList.Add(tee.Type);
                    paramList.Add(tee.Materiaal);
                    paramList.Add(tee.ProductCode);
                    paramList.Add(tee.Omschrijving);
                    paramList.Add(tee.Beschikbaar);

                    ElementSettings.SetCodraftParametersTee(paramList, this.fittingModel);
                    break;

                case "Transition_Concentrisch":
                case "Transition_Excentrisch":
                    Transition transition = (Transition)fitting;
                    paramList.Add(Math.Round(transition.Buitendiameter_1 / feetToMm, 4));
                    paramList.Add(Math.Round(transition.Buitendiameter_2 / feetToMm, 4));
                    paramList.Add(Math.Round(transition.Lengte / feetToMm, 4));
                    paramList.Add(transition.Uiteinde_1_Type);
                    paramList.Add(transition.Uiteinde_2_Type);
                    paramList.Add(Math.Round(transition.Uiteinde_1_Maat / feetToMm, 4));
                    paramList.Add(Math.Round(transition.Uiteinde_2_Maat / feetToMm, 4));
                    paramList.Add(Math.Round(transition.Uiteinde_1_Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(transition.Uiteinde_2_Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(transition.FlensDikte1 / feetToMm, 4));
                    paramList.Add(Math.Round(transition.FlensDikte2 / feetToMm, 4));
                    paramList.Add(transition.Fabrikant);
                    paramList.Add(transition.Type);
                    paramList.Add(transition.Materiaal);
                    paramList.Add(transition.ProductCode);
                    paramList.Add(transition.Omschrijving);
                    paramList.Add(transition.Beschikbaar);

                    ElementSettings.SetCodraftParametersTransition(paramList, this.fittingModel, this._switchNd, this._excentrisch);
                    break;

                case "Tap":
                    Tap tap = (Tap)fitting;
                    paramList.Add(Math.Round(tap.Buitendiameter / feetToMm, 4));
                    paramList.Add(Math.Round(tap.Lengte / feetToMm, 4));
                    paramList.Add(Math.Round(tap.Lengte_Waarde / feetToMm, 4));
                    paramList.Add(tap.Fabrikant);
                    paramList.Add(tap.Type);
                    paramList.Add(tap.Materiaal);
                    paramList.Add(tap.ProductCode);
                    paramList.Add(tap.Omschrijving);
                    paramList.Add(tap.Beschikbaar);

                    ElementSettings.SetCodraftParametersTap(paramList, this.fittingModel);
                    break;

                default:
                    break;
            }

            if (RememberChoice)
            {
                FileOperations.RememberMe(paramList, rememberMeFile, parameters);
            }

            CloseWindow();
        }

        /// <summary>
        /// Open the current database in access
        /// </summary>
        public void OpenDatabase()
        {
            System.Diagnostics.Process.Start(@"C:\Program Files\Microsoft Office\root\Office16\MSACCESS.EXE");

            System.Threading.Thread.Sleep(1500);

            System.Diagnostics.Process.Start(databaseFilePath);
        }

        public void Refresh()
        {
            Fittings.Clear();
            FillList();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
