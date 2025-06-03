using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using ExcelDataReader;
using CodraftPlugin_Library;

namespace CodraftPlugin_Library
{
    public static class Utilities
    {
        public static DataTable GetInsulationData(Document doc)
        {
            DataSet data;

            using (var stream = File.Open(GetPath(doc), FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    data = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                }
            }

            return data.Tables[0];
        }

        private static string GetPath(Document doc)
        {
            var projectPath = GlobalParameters.GetGlobalParameter(doc, "RevitProjectMap");
            var ExcelPath = projectPath + @"\RevitTextFiles\Isolatie.xlsx";

            return ExcelPath;
        }
    }
}
