using Autodesk.Revit.DB;

namespace CodraftPlugin_Updaters.FittingTypes
{
    public class BaseFitting
    {
        public string ConnectionString { get; private set; }
        public string TextFilesMapPath { get; set; }
        public string DatabaseFilePath { get; set; }
        public string RememberMeFile { get; set; }
        public string DatabaseFile { get; set; }
        public FamilyInstance Fi { get; set; }
        public Document Doc { get; set; }
        public ElementId Id { get; set; }
        public string SystemType { get; set; }

        public BaseFitting(FamilyInstance fitting, Document doc, string databaseMapPath, string textFilesMapPath)
        {
            this.Fi = fitting;
            this.Doc = doc;
            this.Id = fitting.Id;
            this.SystemType = fitting.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString();

            this.TextFilesMapPath = textFilesMapPath;
            this.RememberMeFile = textFilesMapPath + "RememberMe_New.txt";
            this.DatabaseFile = this.SystemType + ".mdb";
            this.ConnectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={databaseMapPath}{this.DatabaseFile}";
            this.DatabaseFilePath = databaseMapPath + this.DatabaseFile;
        }
    }
}
