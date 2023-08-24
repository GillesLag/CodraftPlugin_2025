using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CodraftPlugin_UIDatabaseWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class UIDatabase : Window
    {
        public UIDatabase(string connectionString, string strSQL, Document doc, FamilyInstance fitting, string textFilesMapPath, string databaseFilePath, string rememberMeFilePath,
            List<string> parameters, JObject file, bool switchNd = false, int excentrisch = 0, double maxDiameter = 0)
        {
            InitializeComponent();
            ModelView.FittingModelView mv = new ModelView.FittingModelView(connectionString, databaseFilePath, rememberMeFilePath, strSQL, fitting, parameters,
                switchNd, excentrisch, file, maxDiameter);

            DataContext = mv;
            if (mv.CloseWindow == null)
                mv.CloseWindow = new Action(this.Close);
        }

        private void fdgCataloog_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyDescriptor propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
            e.Column.Header = propertyDescriptor.DisplayName;
            if (propertyDescriptor.DisplayName == "Id" || propertyDescriptor.DisplayName == "Fitting")
            {
                e.Cancel = true;
            }
        }
    }
}
