using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace CodraftPlugin_Library
{
    public static class GlobalParameters
    {
        public static string SetGlobalParameter(Document doc, string globalParameterName)
        {
            if (!GlobalParametersManager.AreGlobalParametersAllowed(doc))
            {
                return "";
            }

            ElementId parameterId = GlobalParametersManager.FindByName(doc, globalParameterName);

            if (parameterId != ElementId.InvalidElementId)
            {
                GlobalParameter gp = (GlobalParameter)doc.GetElement(parameterId);

                string gpValue = ((StringParameterValue)gp.GetValue()).Value;
                string path = GetProjectFolderPath(doc);

                if (gpValue == path)
                {
                    return "";
                }

                gp.SetValue(new StringParameterValue(path));

                return path;
            }
            
            GlobalParameter parameter = GlobalParameter.Create(doc, globalParameterName, SpecTypeId.String.Text);
            try
            {
                string path = GetProjectFolderPath(doc);
                parameter.SetValue(new StringParameterValue(path));
                return path;
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static string GetProjectFolderPath(Document doc)
        {
            ModelPath centralModelPath = null;

            try
            {
                centralModelPath = doc.GetWorksharingCentralModelPath();
            }
            catch (Exception)
            {

            }

            string username = WindowsIdentity.GetCurrent().Name.Split('\\')[1];

            if (centralModelPath != null)
            {
                string path = ModelPathUtils.ConvertModelPathToUserVisiblePath(centralModelPath);
                path = path.Substring(0, path.LastIndexOf('\\'));

                return GetPathName(path, username);
            }

            string modelPath = doc.PathName.Substring(0, doc.PathName.LastIndexOf('\\'));
            return GetPathName(modelPath, username);
        }

        private static string GetPathName(string fullPath, string username)
        {
            if (fullPath.Contains(username))
            {
                return fullPath.Replace(username, "(user)");
            }

            return fullPath;
        }
    }
}
