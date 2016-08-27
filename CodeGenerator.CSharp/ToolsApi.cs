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

        private static string AccessCommonUtilsContent
        {
            get
            {
                if (null == _accessCommonUtilsContent)
                    _accessCommonUtilsContent = RessourceApi.ReadString("Tools.Access.CommonUtils.txt");
                return _accessCommonUtilsContent;
            }
        }
        private static string _accessCommonUtilsContent;

        private static string AccessDocumentFormatContent
        {
            get
            {
                if (null == _accessDocumentFormatContent)
                    _accessDocumentFormatContent = RessourceApi.ReadString("Tools.Access.Utils.DocumentFormat.txt");
                return _accessDocumentFormatContent;
            }
        }
        private static string _accessDocumentFormatContent;

        private static string AccessFileUtilsContent
        {
            get
            {
                if (null == _accessFileUtilsContent)
                    _accessFileUtilsContent = RessourceApi.ReadString("Tools.Access.Utils.FileUtils.txt");
                return _accessFileUtilsContent;
            }
        }
        private static string _accessFileUtilsContent;

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

        public static string ExcelCOMAddinDescriptorContent
        {
            get
            {
                if (null == _excelCOMAddinDescriptorContent)
                    _excelCOMAddinDescriptorContent = RessourceApi.ReadString("Tools.Excel.COMAddinTypeDescriptionProvider.txt");
                return _excelCOMAddinDescriptorContent;
            }
        }
        private static string _excelCOMAddinDescriptorContent;

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

        private static string ExcelCommonUtilsContent
        {
            get
            {
                if (null == _excelCommonUtilsContent)
                    _excelCommonUtilsContent = RessourceApi.ReadString("Tools.Excel.CommonUtils.txt");
                return _excelCommonUtilsContent;
            }
        }
        private static string _excelCommonUtilsContent;

        private static string ExcelDocumentFormatContent
        {
            get
            {
                if (null == _excelDocumentFormatContent)
                    _excelDocumentFormatContent = RessourceApi.ReadString("Tools.Excel.Utils.DocumentFormat.txt");
                return _excelDocumentFormatContent;
            }
        }
        private static string _excelDocumentFormatContent;

        private static string ExcelFileUtilsContent
        {
            get
            {
                if (null == _excelFileUtilsContent)
                    _excelFileUtilsContent = RessourceApi.ReadString("Tools.Excel.Utils.FileUtils.txt");
                return _excelFileUtilsContent;
            }
        }
        private static string _excelFileUtilsContent;

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

        private static string OfficeCommonUtilsContent
        {
            get
            {
                if (null == _officeCommonUtilsContent)
                    _officeCommonUtilsContent = RessourceApi.ReadString("Tools.Office.CommonUtils.txt");
                return _officeCommonUtilsContent;
            }
        }
        private static string _officeCommonUtilsContent;

        private static string OfficeColorUtilsContent
        {
            get
            { 
                if(null == _officeColorUtilsContent)
                    _officeColorUtilsContent = RessourceApi.ReadString("Tools.Office.Utils.ColorUtils.txt");
                return _officeColorUtilsContent;
            }
        }
        private static string _officeColorUtilsContent;

        private static string OfficeDialogUtilsContent
        {
            get
            {
                if (null == _officeDialogUtilsContent)
                    _officeDialogUtilsContent = RessourceApi.ReadString("Tools.Office.Utils.DialogUtils.txt");
                return _officeDialogUtilsContent;
            }
        }
        private static string _officeDialogUtilsContent;

        private static string OfficeImageUtilsContent
        {
            get
            {
                if (null == _officeImageUtilsContent)
                    _officeImageUtilsContent = RessourceApi.ReadString("Tools.Office.Utils.ImageUtils.txt");
                return _officeImageUtilsContent;
            }
        }
        private static string _officeImageUtilsContent;

        private static string OfficeResourceUtilsContent
        {
            get
            {
                if (null == _officeResourceUtilsContent)
                    _officeResourceUtilsContent = RessourceApi.ReadString("Tools.Office.Utils.ResourceUtils.txt");
                return _officeResourceUtilsContent;
            }
        }
        private static string _officeResourceUtilsContent;

        private static string OfficeTrayUtilsContent
        {
            get
            {
                if (null == _officeTrayUtilsContent)
                    _officeTrayUtilsContent = RessourceApi.ReadString("Tools.Office.Utils.TrayUtils.txt");
                return _officeTrayUtilsContent;
            }
        }
        private static string _officeTrayUtilsContent;

        private static string OfficeAppDomainInfoContent
        {
            get 
            {
                if (null == _officeAppDomainInfoContent)
                    _officeAppDomainInfoContent = RessourceApi.ReadString("Tools.Office.Informations.AppDomainInfo.txt");
                return _officeAppDomainInfoContent;
            }
        }
        private static string _officeAppDomainInfoContent;

        private static string OfficeAssemblyInfoContent
        {
            get
            {
                if (null == _officeAssemblyInfoContent)
                    _officeAssemblyInfoContent = RessourceApi.ReadString("Tools.Office.Informations.AssemblyInfo.txt");
                return _officeAssemblyInfoContent;
            }
        }
        private static string _officeAssemblyInfoContent;

        private static string OfficeDiagnosticPairContent
        {
            get 
            {
                if (null == _officeDiagnosticPairContent)
                    _officeDiagnosticPairContent = RessourceApi.ReadString("Tools.Office.Informations.DiagnosticPair.txt");
                return _officeDiagnosticPairContent;
            }
        }
        private static string _officeDiagnosticPairContent;

        private static string OfficeDiagnosticPairCollectionContent
        {
            get
            {
                if (null == _officeDiagnosticPairCollectionContent)
                    _officeDiagnosticPairCollectionContent = RessourceApi.ReadString("Tools.Office.Informations.DiagnosticPairCollection.txt");
                return _officeDiagnosticPairCollectionContent;
            }
        }
        private static string _officeDiagnosticPairCollectionContent;

        private static string OfficeEnvironmentInfoContent
        {
            get
            {
                if (null == _officeEnvrironmentInfoContent)
                    _officeEnvrironmentInfoContent = RessourceApi.ReadString("Tools.Office.Informations.EnvironmentInfo.txt");
                return _officeEnvrironmentInfoContent;
            }
        }
        private static string _officeEnvrironmentInfoContent;

        private static string OfficeHostInfoContent
        {
            get
            {
                if (null == _officeHostInfoContent)
                    _officeHostInfoContent = RessourceApi.ReadString("Tools.Office.Informations.HostInfo.txt");
                return _officeHostInfoContent;
            }
        }
        private static string _officeHostInfoContent;

        private static string OfficeInfosContent
        {
            get
            {
                if (null == _officeInfosContent)
                    _officeInfosContent = RessourceApi.ReadString("Tools.Office.Informations.Infos.txt");
                return _officeInfosContent;
            }

        }
        private static string _officeInfosContent;

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

        private static string OutlookCommonUtilsContent
        {
            get
            {
                if (null == _outlookCommonUtilsContent)
                    _outlookCommonUtilsContent = RessourceApi.ReadString("Tools.Outlook.CommonUtils.txt");
                return _outlookCommonUtilsContent;
            }
        }
        private static string _outlookCommonUtilsContent;

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

        private static string PointCommonUtilsContent
        {
            get
            {
                if (null == _pointCommonUtilsContent)
                    _pointCommonUtilsContent = RessourceApi.ReadString("Tools.PowerPoint.CommonUtils.txt");
                return _pointCommonUtilsContent;
            }
        }
        private static string _pointCommonUtilsContent;

        private static string PointDocumentFormatContent
        {
            get
            {
                if (null == _pointDocumentFormatContent)
                    _pointDocumentFormatContent = RessourceApi.ReadString("Tools.PowerPoint.Utils.DocumentFormat.txt");
                return _pointDocumentFormatContent;
            }
        }
        private static string _pointDocumentFormatContent;

        private static string PointFileUtilsContent
        {
            get
            {
                if (null == _pointFileUtilsContent)
                    _pointFileUtilsContent = RessourceApi.ReadString("Tools.PowerPoint.Utils.FileUtils.txt");
                return _pointFileUtilsContent;
            }
        }
        private static string _pointFileUtilsContent;

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

        private static string ProjectCommonUtilsContent
        {
            get
            {
                if (null == _projectCommonUtilsContent)
                    _projectCommonUtilsContent = RessourceApi.ReadString("Tools.Project.CommonUtils.txt");
                return _projectCommonUtilsContent;
            }
        }
        private static string _projectCommonUtilsContent;

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

        private static string WordCommonUtilsContent
        {
            get
            {
                if (null == _wordCommonUtilsContent)
                    _wordCommonUtilsContent = RessourceApi.ReadString("Tools.Word.CommonUtils.txt");
                return _wordCommonUtilsContent;
            }
        }
        private static string _wordCommonUtilsContent;

        private static string WordDocumentFormatContent
        {
            get
            {
                if (null == _wordDocumentFormatContent)
                    _wordDocumentFormatContent = RessourceApi.ReadString("Tools.Word.Utils.DocumentFormat.txt");
                return _wordDocumentFormatContent;
            }
        }
        private static string _wordDocumentFormatContent;

        private static string WordFileUtilsContent
        {
            get
            {
                if (null == _wordFileUtilsContent)
                    _wordFileUtilsContent = RessourceApi.ReadString("Tools.Word.Utils.FileUtils.txt");
                return _wordFileUtilsContent;
            }
        }
        private static string _wordFileUtilsContent;

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

            PathApi.CreateFolder(faceFolder + "\\Utils");

            string file3 = AccessCommonUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "CommonUtils.cs"), file3);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "CommonUtils.cs" + "\" />" + "\r\n";

            string file4 = AccessDocumentFormatContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\DocumentFormat.cs"), file4);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "DocumentFormat.cs" + "\" />" + "\r\n";

            string file5 = AccessFileUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\FileUtils.cs"), file5);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "FileUtils.cs" + "\" />" + "\r\n";

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

            PathApi.CreateFolder(faceFolder + "\\Utils");

            string file3 = ExcelCommonUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "CommonUtils.cs"), file3);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "CommonUtils.cs" + "\" />" + "\r\n";

            string file4 = ExcelDocumentFormatContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\DocumentFormat.cs"), file4);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "DocumentFormat.cs" + "\" />" + "\r\n";

            string file5 = ExcelFileUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\FileUtils.cs"), file5);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "FileUtils.cs" + "\" />" + "\r\n";

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

            PathApi.CreateFolder(faceFolder + "\\Dialogs");
            PathApi.CreateFolder(faceFolder + "\\Informations");
            PathApi.CreateFolder(faceFolder + "\\Utils");

            string file5 = OfficeCommonUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "CommonUtils.cs"), file5);            
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "CommonUtils.cs" + "\" />" + "\r\n";

            string file6 = OfficeColorUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\ColorUtils.cs"), file6);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "ColorUtils.cs" + "\" />" + "\r\n";

            string file7 = OfficeDialogUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\DialogUtils.cs"), file7);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "DialogUtils.cs" + "\" />" + "\r\n";

            string file8 = OfficeImageUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\ImageUtils.cs"), file8);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "ImageUtils.cs" + "\" />" + "\r\n";

            string file9 = OfficeResourceUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\ResourceUtils.cs"), file9);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "ResourceUtils.cs" + "\" />" + "\r\n";

            string file10 = OfficeTrayUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\TrayUtils.cs"), file10);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "TrayUtils.cs" + "\" />" + "\r\n";

            string file11 = OfficeAppDomainInfoContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Informations\\AppDomainInfo.cs"), file11);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Informations\\" + "AppDomainInfo.cs" + "\" />" + "\r\n";

            string file12 = OfficeAssemblyInfoContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Informations\\AssemblyInfo.cs"), file12);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Informations\\" + "AssemblyInfo.cs" + "\" />" + "\r\n";

            string file13 = OfficeDiagnosticPairContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Informations\\DiagnosticPair.cs"), file13);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Informations\\" + "DiagnosticPair.cs" + "\" />" + "\r\n";

            string file14 = OfficeDiagnosticPairCollectionContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Informations\\DiagnosticPairCollection.cs"), file14);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Informations\\" + "DiagnosticPairCollection.cs" + "\" />" + "\r\n";

            string file15 = OfficeEnvironmentInfoContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Informations\\EnvironmentInfo.cs"), file15);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Informations\\" + "EnvironmentInfo.cs" + "\" />" + "\r\n";

            string file16 = OfficeHostInfoContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Informations\\HostInfo.cs"), file16);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Informations\\" + "HostInfo.cs" + "\" />" + "\r\n";

            string file17 = OfficeInfosContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Informations\\Infos.cs"), file17);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Informations\\" + "Infos.cs" + "\" />" + "\r\n";

            string file18 = RessourceApi.ReadString("Tools.Office.Dialogs.DialogLayoutSettings.txt");
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\DialogLayoutSettings.cs"), file18);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + "DialogLayoutSettings.cs" + "\" />" + "\r\n";

            string file19 = RessourceApi.ReadString("Tools.Office.Dialogs.DialogLocalizationSettings.txt");
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\DialogLocalizationSettings.cs"), file19);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + "DialogLocalizationSettings.cs" + "\" />" + "\r\n";


            string[] dialogs = new string[] { "AboutDialog", "DiagnosticsDialog", "ErrorDialog", "RichTextDialog"};
            foreach (string item in dialogs)
            {
                string file = String.Format("{0}{1}", item, ".cs");
                string designer = String.Format("{0}{1}", item, ".designer.cs");
                string res = String.Format("{0}{1}", item, ".resx");
                string lang1031 = String.Format("{0}{1}", item, ".1031.xml");
                string lang1033 = String.Format("{0}{1}", item, ".1033.xml");

                string contentFile = RessourceApi.ReadString("Tools.Office.Dialogs." + item + ".txt");
                string contentDesignerFile = RessourceApi.ReadString("Tools.Office.Dialogs." + item + ".Designer.txt");
                string contentResFile = RessourceApi.ReadString("Tools.Office.Dialogs." + item + ".rtxt");

                System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\" + item +".cs"), contentFile);
                System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\" + item + ".designer.cs"), contentDesignerFile);
                System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\" + item +".resx"), contentResFile);


                result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + item + ".cs" + "\">" + "\r\n" +
                            "\t\t\t<SubType>Form</SubType>" + "\r\n" +
                            "\t\t</Compile>" + "\r\n";

                result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + item + ".designer.cs" + "\">" + "\r\n" +
                           "\t\t\t<DependentUpon>" +  item + ".cs" + "</DependentUpon>" + "\r\n" +
                           "\t\t</Compile>" + "\r\n";


                result += "\t\t<EmbeddedResource Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + item + ".resx" + "\">" + "\r\n" +
                       "\t\t\t<DependentUpon>" + item + ".cs" + "</DependentUpon>" + "\r\n" +
                       "\t\t</EmbeddedResource>" + "\r\n";

                System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\" + lang1031), contentFile);
                System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\" + lang1033), contentFile);

                result += "\t\t<EmbeddedResource Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + lang1031 + "\"/>" + "\r\n";
                result += "\t\t<EmbeddedResource Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + lang1033 + "\"/>" + "\r\n";
            }


            string baseFile = RessourceApi.ReadString("Tools.Office.Dialogs.ToolsDialog.txt");
            string baseDesign = RessourceApi.ReadString("Tools.Office.Dialogs.ToolsDialog.Designer.txt");
            string baseRes = RessourceApi.ReadString("Tools.Office.Dialogs.ToolsDialog.rtxt");
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\ToolsDialog.cs"), baseFile);
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\ToolsDialog.designer.cs"), baseDesign);
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Dialogs\\ToolsDialog.resx"), baseRes);

            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + "ToolsDialog.cs" + "\">" + "\r\n" +
            "\t\t\t<SubType>Form</SubType>" + "\r\n" +
            "\t\t</Compile>" + "\r\n";

            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + "ToolsDialog.designer.cs" + "\">" + "\r\n" +
                       "\t\t\t<DependentUpon>" + "ToolsDialog.cs" + "</DependentUpon>" + "\r\n" +
                       "\t\t</Compile>" + "\r\n";


            result += "\t\t<EmbeddedResource Include=\"" + faceFolder.Substring(i + 1) + "\\Dialogs\\" + "ToolsDialog.resx" + "\">" + "\r\n" +
                   "\t\t\t<DependentUpon>" + "ToolsDialog.cs" + "</DependentUpon>" + "\r\n" +
                   "\t\t</EmbeddedResource>" + "\r\n";



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

            string file3 = OutlookCommonUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "CommonUtils.cs"), file3);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "CommonUtils.cs" + "\" />" + "\r\n";
 
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

            PathApi.CreateFolder(faceFolder + "\\Utils");

            string file3 = PointCommonUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "CommonUtils.cs"), file3);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "CommonUtils.cs" + "\" />" + "\r\n";

            string file4 = PointDocumentFormatContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\DocumentFormat.cs"), file4);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "DocumentFormat.cs" + "\" />" + "\r\n";

            string file5 = PointFileUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\FileUtils.cs"), file5);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "FileUtils.cs" + "\" />" + "\r\n";

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

            string file3 = ProjectCommonUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "CommonUtils.cs"), file3);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "CommonUtils.cs" + "\" />" + "\r\n";

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

            PathApi.CreateFolder(faceFolder + "\\Utils");

            string file3 = WordCommonUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "CommonUtils.cs"), file3);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\" + "CommonUtils.cs" + "\" />" + "\r\n";

            string file4 = WordDocumentFormatContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\DocumentFormat.cs"), file4);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "DocumentFormat.cs" + "\" />" + "\r\n";

            string file5 = WordFileUtilsContent;
            System.IO.File.AppendAllText(System.IO.Path.Combine(faceFolder, "Utils\\FileUtils.cs"), file5);
            result += "\t\t<Compile Include=\"" + faceFolder.Substring(i + 1) + "\\Utils\\" + "FileUtils.cs" + "\" />" + "\r\n";

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
