using Autodesk.Revit.DB;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodraftPlugin_Updaters.PipeAccessoriesTypes
{
    public abstract class BaseAccessory
    {
        public List<string> CallingParams = new List<string>();

        public const double feetToMm = 304.8;
        public string ConnectionString { get; set; }
        public string DatabaseFilePath { get; set; }
        public string DatabaseFile { get; set; }
        public double Dn { get; private set; }
        public string Fabrikant { get; private set; }
        public string Type { get; private set; }
        public string Query { get; set; }
        public string QueryCount { get; set; }
        public string RememberMeFilePath { get; set; }
        public List<object> DatabaseParameters { get; set; }
        public List<object> RevitParameters { get; set; } = new List<object>();
        public FamilyInstance PipeAccessory { get; set; }
        public Document Doc { get; set; }
        public JObject parameterConfiguration { get; set; }

        public BaseAccessory(FamilyInstance accessory, Document doc, string databaseMapPath, JObject file)
        {
            this.PipeAccessory = accessory;
            this.Doc = doc;
            this.DatabaseFile = accessory.Symbol.FamilyName + ".accdb";
            this.DatabaseFilePath = databaseMapPath + this.DatabaseFile;
            this.ConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={databaseMapPath}{this.DatabaseFile}";

            string revitFilePath = PipeAccessory.Document.PathName;
            this.RememberMeFilePath = revitFilePath.Substring(0, revitFilePath.LastIndexOf('\\') + 1) + "RevitTextFiles\\RememberMePipeAccessories.txt";

            ConnectorSetIterator iterator = accessory.MEPModel.ConnectorManager.Connectors.ForwardIterator();

            while (iterator.MoveNext())
            {
                Connector connector = (Connector)iterator.Current;
                this.Dn = Math.Round(connector.Radius * 2 * feetToMm, 2);
                break;
            }

            string fabrikantType = accessory.Name.Substring(0, accessory.Name.IndexOf('%'));

            this.Fabrikant = fabrikantType.Substring(0, fabrikantType.IndexOf('_'));
            this.Type = fabrikantType.Substring(fabrikantType.IndexOf('_') + 1);

            CallingParams.Add(this.Fabrikant);
            CallingParams.Add(this.Type);
            CallingParams.Add(this.Dn.ToString());

            parameterConfiguration = file;
        }

        public abstract bool? GetParams();
        public abstract bool ParametersAreTheSame();
        public abstract void SetWrongValues();
    }
}
