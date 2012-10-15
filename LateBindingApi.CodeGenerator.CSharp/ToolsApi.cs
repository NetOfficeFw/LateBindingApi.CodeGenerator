using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.CSharp
{
    internal static class ToolsApi
    {
        private static  string[] _targets = new string[] { "Office", "Excel", "Word", "Outlook", "PowerPoint", "Access", "MSProject", "Visio" };

        private static bool IsTarget(string name)
        {
            foreach (var item in _targets)
            {
                if (item.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        private static string CustomTaskPaneCollectionContent
        {
            get
            {
                if (null == _customTaskPaneCollection)
                    _customTaskPaneCollection = RessourceApi.ReadString("Tools.Office.CustomTaskPaneCollection.txt");
                return _customTaskPaneCollection;
            }
        }
        private static string _customTaskPaneCollection;

        #region Access Ressources

        private static string AccessCOMAddinContent
        {
            get
            {
                if (null == _accessCOMAddinContent)
                    _accessCOMAddinContent = RessourceApi.ReadString("Tools.Access.COMAddin.txt");
                return _accessCOMAddinContent;
            }
        }
        private static string _accessCOMAddinContent;

        private static string AccessTaskPaneContent
        {
            get
            {
                if (null == _accessTaskPaneContent)
                    _accessTaskPaneContent = RessourceApi.ReadString("Tools.Access.ITaskPane.txt");
                return _accessTaskPaneContent;
            }
        }
        private static string _accessTaskPaneContent;

        #endregion

        #region Excel Ressources

        private static string ExcelCOMAddinContent
        {
            get
            {
                if (null == _excelCOMAddinContent)
                    _excelCOMAddinContent = RessourceApi.ReadString("Tools.Excel.COMAddin.txt");
                return _excelCOMAddinContent;
            }
        }
        private static string _excelCOMAddinContent;

        private static string ExcelTaskPaneContent
        {
            get
            {
                if (null == _excelTaskPaneContent)
                    _excelTaskPaneContent = RessourceApi.ReadString("Tools.Excel.ITaskPane.txt");
                return _excelTaskPaneContent;
            }
        }
        private static string _excelTaskPaneContent;

        #endregion

        #region Visio Ressources

        private static string VisioCOMAddinContent
        {
            get
            {
                if (null == _visioCOMAddinContent)
                    _visioCOMAddinContent = RessourceApi.ReadString("Tools.Visio.COMAddin.txt");
                return _visioCOMAddinContent;
            }
        }
        private static string _visioCOMAddinContent;

        #endregion

        #region Office Ressources

        private static string OfficeAttributeContent
        {
            get
            {
                if (null == _officeAttributeContent)
                    _officeAttributeContent = RessourceApi.ReadString("Tools.Office.MultiRegisterAttribute.txt");
                return _officeAttributeContent;
            }
        }
        private static string _officeAttributeContent;

        private static string OfficeCOMAddinContent
        {
            get
            {
                if (null == _officeCOMAddinContent)
                    _officeCOMAddinContent = RessourceApi.ReadString("Tools.Office.COMAddin.txt");
                return _officeCOMAddinContent;
            }
        }
        private static string _officeCOMAddinContent;

        private static string OfficeTaskPaneContent
        {
            get
            {
                if (null == _officeTaskPaneContent)
                    _officeTaskPaneContent = RessourceApi.ReadString("Tools.Office.ITaskPane.txt");
                return _officeTaskPaneContent;
            }
        }
        private static string _officeTaskPaneContent;

        private static string OfficeCustomTaskPaneCollectionContent
        {
            get
            {
                if (null == _officeCustomTaskPaneContent)
                    _officeCustomTaskPaneContent = RessourceApi.ReadString("Tools.Office.CustomTaskPaneCollection.txt");
                return _officeCustomTaskPaneContent;
            }
        }
        private static string _officeCustomTaskPaneContent;


        #endregion

        #region Outlook Ressources

        private static string OutlookCOMAddinContent
        {
            get
            {
                if (null == _outlookCOMAddinContent)
                    _outlookCOMAddinContent = RessourceApi.ReadString("Tools.Outlook.COMAddin.txt");
                return _outlookCOMAddinContent;
            }
        }
        private static string _outlookCOMAddinContent;

        private static string OutlookTaskPaneContent
        {
            get
            {
                if (null == _outlookTaskPaneContent)
                    _outlookTaskPaneContent = RessourceApi.ReadString("Tools.Outlook.ITaskPane.txt");
                return _outlookTaskPaneContent;
            }
        }
        private static string _outlookTaskPaneContent;

        #endregion

        #region PowerPoint Ressources

        private static string PowerPointCOMAddinContent
        {
            get
            {
                if (null == _pointCOMAddinContent)
                    _pointCOMAddinContent = RessourceApi.ReadString("Tools.PowerPoint.COMAddin.txt");
                return _pointCOMAddinContent;
            }
        }
        private static string _pointCOMAddinContent;

        private static string PowerPointTaskPaneContent
        {
            get
            {
                if (null == _pointTaskPaneContent)
                    _pointTaskPaneContent = RessourceApi.ReadString("Tools.PowerPoint.ITaskPane.txt");
                return _pointTaskPaneContent;
            }
        }
        private static string _pointTaskPaneContent;

        #endregion

        #region Project Ressources

        private static string ProjectCOMAddinContent
        {
            get
            {
                if (null == _projectCOMAddinContent)
                    _projectCOMAddinContent = RessourceApi.ReadString("Tools.Project.COMAddin.txt");
                return _projectCOMAddinContent;
            }
        }
        private static string _projectCOMAddinContent;

        private static string ProjectTaskPaneContent
        {
            get
            {
                if (null == _projectTaskPaneContent)
                    _projectTaskPaneContent = RessourceApi.ReadString("Tools.Project.ITaskPane.txt");
                return _projectTaskPaneContent;
            }
        }
        private static string _projectTaskPaneContent;

        #endregion

        #region Word Ressources

        private static string WordCOMAddinContent
        {
            get
            {
                if (null == _wordCOMAddinContent)
                    _wordCOMAddinContent = RessourceApi.ReadString("Tools.Word.COMAddin.txt");
                return _wordCOMAddinContent;
            }
        }
        private static string _wordCOMAddinContent;

        private static string WordTaskPaneContent
        {
            get
            {
                if (null == _wordTaskPaneContent)
                    _wordTaskPaneContent = RessourceApi.ReadString("Tools.Word.ITaskPane.txt");
                return _wordTaskPaneContent;
            }
        }
        private static string _wordTaskPaneContent;

        #endregion

        internal static string ConvertAccessToolsToFiles(XElement projectNode, Settings settings, string faceFolder)
        {
            string file1 = AccessCOMAddinContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "COMAddin.cs"), file1);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "COMAddin.cs" + "\" />" + "\r\n";

            string file2 = AccessTaskPaneContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "ITaskPane.cs"), file2);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "ITaskPane.cs" + "\" />" + "\r\n";

            return result;
        }

        internal static string ConvertExcelToolsToFiles(XElement projectNode, Settings settings, string faceFolder)
        {
            string file1 = ExcelCOMAddinContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "COMAddin.cs"), file1);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "COMAddin.cs" + "\" />" + "\r\n";

            string file2 = ExcelTaskPaneContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "ITaskPane.cs"), file2);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "ITaskPane.cs" + "\" />" + "\r\n";

            return result;
        }

        internal static string ConvertVisioToolsToFiles(XElement projectNode, Settings settings, string faceFolder)
        {
            string file1 = VisioCOMAddinContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "COMAddin.cs"), file1);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "COMAddin.cs" + "\" />" + "\r\n";

            return result;
        }

        internal static string ConvertOfficeToolsToFiles(XElement projectNode, Settings settings, string faceFolder)
        {
            string file1 = OfficeCOMAddinContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "COMAddin.cs"), file1);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "COMAddin.cs" + "\" />" + "\r\n";

            string file2 = OfficeTaskPaneContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "ITaskPane.cs"), file2);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "ITaskPane.cs" + "\" />" + "\r\n";

            string file3 = OfficeCustomTaskPaneCollectionContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "CustomTaskPaneCollection.cs"), file3);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "CustomTaskPaneCollection.cs" + "\" />" + "\r\n";

            string file4 = OfficeAttributeContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "MultiRegisterAttribute.cs"), file4);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "MultiRegisterAttribute.cs" + "\" />" + "\r\n";

            return result;   
        }
          
        internal static string ConvertOutlookToolsToFiles(XElement projectNode, Settings settings, string faceFolder)
        {
            string file1 = OutlookCOMAddinContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "COMAddin.cs"), file1);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "COMAddin.cs" + "\" />" + "\r\n";

            string file2 = OutlookTaskPaneContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "ITaskPane.cs"), file2);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "ITaskPane.cs" + "\" />" + "\r\n";

            return result;
        }

        internal static string ConvertPowerPointToolsToFiles(XElement projectNode, Settings settings, string faceFolder)
        {
            string file1 = PowerPointCOMAddinContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "COMAddin.cs"), file1);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "COMAddin.cs" + "\" />" + "\r\n";

            string file2 = PowerPointTaskPaneContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "ITaskPane.cs"), file2);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "ITaskPane.cs" + "\" />" + "\r\n";

            return result;
        }

        internal static string ConvertProjectToolsToFiles(XElement projectNode, Settings settings, string faceFolder)
        {
            string file1 = ProjectCOMAddinContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "COMAddin.cs"), file1);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "COMAddin.cs" + "\" />" + "\r\n";

            string file2 = ProjectTaskPaneContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "ITaskPane.cs"), file2);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "ITaskPane.cs" + "\" />" + "\r\n";

            return result;
        }

        internal static string ConvertWordToolsToFiles(XElement projectNode, Settings settings, string faceFolder)
        {
            string file1 = WordCOMAddinContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "COMAddin.cs"), file1);

            int i = faceFolder.LastIndexOf("\\");
            string result = "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "COMAddin.cs" + "\" />" + "\r\n";

            string file2 = WordTaskPaneContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "ITaskPane.cs"), file2);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "ITaskPane.cs" + "\" />" + "\r\n";

            return result;
        }          

        internal static string ConvertToolsToFiles(XElement projectNode,  Settings settings, string solutionFolder)
        {
            if(!IsTarget(projectNode.Attribute("Name").Value))
                return "";

            string faceFolder = System.IO.Path.Combine(solutionFolder, projectNode.Attribute("Name").Value);
            faceFolder = System.IO.Path.Combine(faceFolder, "Tools");
            if (false == System.IO.Directory.Exists(faceFolder))
                System.IO.Directory.CreateDirectory(faceFolder);

            switch (projectNode.Attribute("Name").Value)            
            {
                case "Access":
                    return ConvertAccessToolsToFiles(projectNode, settings, faceFolder);
                case "Excel":
                    return ConvertExcelToolsToFiles(projectNode, settings, faceFolder);
                case "MSProject":
                    return ConvertProjectToolsToFiles(projectNode, settings, faceFolder);
                case "Office":
                    return ConvertOfficeToolsToFiles(projectNode, settings, faceFolder);
                case "Outlook":
                    return ConvertOutlookToolsToFiles(projectNode, settings, faceFolder);
                case "PowerPoint":
                    return ConvertPowerPointToolsToFiles(projectNode, settings, faceFolder);
                case "Visio":
                    return ConvertVisioToolsToFiles(projectNode, settings, faceFolder);
                case "Word":
                    return ConvertWordToolsToFiles(projectNode, settings, faceFolder);
                default:
                    throw new ArgumentOutOfRangeException(projectNode.Attribute("Name").Value);
            }
        }
    }
}
