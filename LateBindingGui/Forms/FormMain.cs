using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Threading;

using LateBindingApi.CodeGenerator.ComponentAnalyzer;
 
namespace LateBindingApi.CodeGenerator.WFApplication
{
    public partial class FormMain : Form
    {
        private const string FileDialog_ProjectFileFilter = @"Project Files (*.xml)|*.xml";

        #region Fields

        Analyzer       _comAnalyzer = new Analyzer();
        ICodeGenerator _generator;
        string         _commandLine;
        CancellationTokenSource _generatorCancellationTokenSource;
        
        #endregion

        #region Construction

        public FormMain()
        {
            InitializeComponent();
            this.Text = this.GetType().Assembly.GetName().Name;
            _comAnalyzer.Update += new UpdateHandler(comAnalyzer_Update);
            _comAnalyzer.Finish += new FinishHandler(comAnalyzer_OnTypeLibrariesLoaded);

            foreach (Control item in splitContainerMain.Panel2.Controls)
                item.Dock = DockStyle.Fill;

            libraryGrid.Initialize(_comAnalyzer.Schema);
            projectGrid.Initialize(_comAnalyzer.Schema);
            interfaceGrid.Initialize(_comAnalyzer.Schema);
            constantGrid.Initialize(_comAnalyzer.Schema);
            enumGrid.Initialize(_comAnalyzer.Schema);
            classGrid.Initialize(_comAnalyzer.Schema);
            solutionGrid.Initialize(_comAnalyzer.Schema);
            modulGrid.Initialize(_comAnalyzer.Schema);
            aliasGrid.Initialize(_comAnalyzer.Schema);
            recordGrid.Initialize(_comAnalyzer.Schema);

            _commandLine = Environment.CommandLine;
        }
     
        #endregion

        #region Menu Click Trigger
       
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                FormAbout about = new FormAbout();
                about.ShowDialog(this);
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        private void websiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/NetOfficeFw");
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }
       
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this); 
            }
        }
        
        private void menuItemLoadTypeLibrary_Click(object sender, EventArgs e)
        {
            FormTypeLibBrowser formBrowser =null;
            try
            {
                formBrowser = new FormTypeLibBrowser();
                this.Refresh();

                if (DialogResult.OK == formBrowser.ShowDialog(this))
                {
                    //invisible all left&right panel content
                    foreach (Control itemControl in splitContainerMain.Panel2.Controls)
                        itemControl.Visible = false;

                    foreach (Control itemControl in splitContainerMain.Panel1.Controls)
                        itemControl.Visible = false;

                    SetGui(false);
                    _comAnalyzer.LoadTypeLibraries(formBrowser.SelectedFiles, formBrowser.AddToCurrentProject, formBrowser.DoAsync);
                    InvisiblePanels();
                }
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
            finally
            {
                SetGui(true);
            }
        }

        private void menuItemLoadProject_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = FileDialog_ProjectFileFilter;
                if (DialogResult.OK == fileDialog.ShowDialog(this))
                {
                    this.Cursor = Cursors.WaitCursor;
                    DateTime startTime = DateTime.Now;
                    _comAnalyzer.LoadProject(fileDialog.FileName);
                    TimeSpan timeElapsed = DateTime.Now - startTime;
                    comAnalyzer_OnTypeLibrariesLoaded(timeElapsed);
                    InvisiblePanels();
                }
            }
            catch (ProjectFileFormatException throwedException)
            {
                FormShowError formError = new FormShowError(throwedException, "Project file invalid or old version.");
                formError.ShowDialog(this);
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }                
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void menuItemSaveProject_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = FileDialog_ProjectFileFilter;
                if (DialogResult.OK == fileDialog.ShowDialog(this))
                {
                    this.Cursor = Cursors.WaitCursor;
                    _comAnalyzer.SaveProject(fileDialog.FileName);
                    MessageBox.Show("Project successfully saved.", this.GetType().Assembly.GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }

        }

        private async void menuItemGenerateCode_Click(object sender, EventArgs e)
        {
            try
            {
                FormGeneratorBrowser formBrowser = new FormGeneratorBrowser();
                if (DialogResult.OK == formBrowser.ShowDialog(this))
                {
                    _generator = formBrowser.Selected;
                    if (DialogResult.OK == _generator.ShowConfigDialog(this))
                    {
                        _generator.Progress = new Progress<string>(generator_Progress);
                        SetGui(false);
                        using (_generatorCancellationTokenSource = new CancellationTokenSource())
                        {
                            var token = this._generatorCancellationTokenSource.Token;
                            var elapsed = await _generator.Generate(_comAnalyzer.Document, token);
                            generator_Finish(elapsed);
                        }
                    }
                }
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        void generator_Finish(TimeSpan elapsedTime)
        {
            SetGui(true);
            statusStripMain.Items[0].Text ="Solution generated in " + elapsedTime.ToString(); 
        }

        void generator_Progress(string message)
        {
            if (true == statusStripMain.InvokeRequired)
            {
                statusStripMain.Tag = message;
                statusStripMain.Invoke(new MethodInvoker(UpdateStrip));
            }
            else
            {
                statusStripMain.Items[0].Text = message;
                statusStripMain.Refresh();
            }    
        }

        #endregion

        #region Methods

        private void InvisiblePanels()
        {
            foreach (Control itemControl in splitContainerMain.Panel2.Controls)
                itemControl.Visible = false;
        }

        private void SetGui(bool state)
        {
            menuStripMain.Enabled = state;
            menuItemSaveProject.Enabled = state;
            menuItemGenerateCode.Enabled = state;
            splitContainerMain.Visible = state;
            if (true == state)
                this.Cursor = Cursors.Default;
            else
                this.Cursor = Cursors.WaitCursor;
        }

        private void UpdateStrip()
        {
            try
            {
                statusStripMain.Items[0].Text = statusStripMain.Tag as string;
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        #endregion

        #region Trigger

        private void comAnalyzer_OnTypeLibrariesLoaded(TimeSpan timeElapsed)
        {
            try
            {
                foreach (Control itemControl in splitContainerMain.Panel1.Controls)
                    itemControl.Visible = true;
                libraryTreeBrowser.Show(_comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document"));
                SetGui(true);
                statusStripMain.Items[0].Text = string.Format("Loaded in {0}", timeElapsed);
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        void comAnalyzer_Update(object sender, string message)
        {
            try
            {
                if (true == statusStripMain.InvokeRequired)
                {
                    statusStripMain.Tag = message;
                    statusStripMain.Invoke(new MethodInvoker(UpdateStrip));
                }
                else
                {  
                    statusStripMain.Items[0].Text = message;
                    statusStripMain.Refresh();
                }                
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        private void libraryTreeBrowser_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                //invisible all right panel content
                foreach (Control itemControl in splitContainerMain.Panel2.Controls)
                    itemControl.Visible = false;

                XElement node = e.Node.Tag as XElement;
                if (null == node)
                    return;

                switch (node.Name.LocalName)
                {
                    case "Libraries":
                        libraryGrid.Show(node);
                        libraryGrid.Visible = true;
                        break;
                    case "Solution":
                        solutionGrid.Show(node);
                        solutionGrid.Visible = true;
                        break;
                    case "Project":
                        projectGrid.Show(node);
                        projectGrid.Visible = true;
                        break;
                    case "Constant":
                        constantGrid.Show(node);
                        constantGrid.Visible = true;
                        break;
                    case "Enum":
                        enumGrid.Show(node);
                        enumGrid.Visible = true;
                        break;
                    case "Interface":
                        interfaceGrid.Show(node);
                        interfaceGrid.Visible = true;
                        break;
                    case "CoClass":
                        classGrid.Show(node);
                        classGrid.Visible = true;
                        break;
                    case "Modul":
                        modulGrid.Show(node);
                        modulGrid.Visible = true;
                        break;
                    case "Alias":
                        aliasGrid.Show(node);
                        aliasGrid.Visible = true;
                        break;
                    case "Record":
                        recordGrid.Show(node);
                        recordGrid.Visible = true;
                        break;
                }

            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                // abort async analyze operation
                if ((true == _comAnalyzer.DoAsync) && (true == _comAnalyzer.IsAlive))
                    _comAnalyzer.Abort();

                // abort async generate operation
                _generatorCancellationTokenSource?.Cancel(true);
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        private void menuItemStatistics_Click(object sender, EventArgs e)
        {
            if (null != _comAnalyzer.Document)
            { 
                FormStatistics form = new FormStatistics(_comAnalyzer.Document);
                form.ShowDialog(this);
            }
        }

        #endregion

        private void toolStripNetOfficeCheats_Click(object sender, EventArgs e)
        {
            try
            {
                if (null != _comAnalyzer.Document)
                {
                    FormNetOffice dialog = new FormNetOffice();
                    if (DialogResult.OK == dialog.ShowDialog(this))
                    {

                        if (dialog.EnableSolutionName)
                        {
                            _comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Attribute("Name").Value = dialog.SolutionName;
                        }

                        if (dialog.EnableAssemblyVersion)
                        {
                            var projects = _comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project");
                            foreach (var item in projects)
                            {
                                item.Attribute("Version").Value = dialog.AssemblyVersion;
                                item.Attribute("FileVersion").Value = dialog.AssemblyVersion;
                            }
                        }

                        if (dialog.EnableNamespaces)
                        {
                            var projects = _comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project");
                            foreach (var item in projects)
                            {
                                if (item.Attribute("Ignore").Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                                    item.Attribute("Namespace").Value = item.Attribute("Name").Value;
                                else
                                    item.Attribute("Namespace").Value = "NetOffice." + item.Attribute("Name").Value + "Api";
                            }
                        }

                        if (dialog.EnableCallQuit)
                        {
                            var projects = _comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project");
                            foreach (XElement item in projects)
                            {
                                if ((item.Attribute("Name").Value == "Excel") || (item.Attribute("Name").Value == "Word") || (item.Attribute("Name").Value == "Outlook") || (item.Attribute("Name").Value == "PowerPoint")  || (item.Attribute("Name").Value == "Access"))
                                {
                                    XElement appNode = (from a in item.Element("CoClasses").Elements("CoClass")
                                                        where a.Attribute("Name").Value.Equals("Application", StringComparison.InvariantCultureIgnoreCase)
                                                        select a).FirstOrDefault();
                                    if (null == appNode)
                                        throw new ArgumentOutOfRangeException("Cant find Application Class ");

                                    appNode.Attribute("AutomaticQuit").Value = "true";
                                }
                            }                          
                        }

                        if (dialog.EnableAnalyzeReturn)
                        {
                            XElement projectNode = (from a in _comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project")
                                                where a.Attribute("Name").Value.Equals("Office", StringComparison.InvariantCultureIgnoreCase)
                                                select a).FirstOrDefault();
                            
                            XElement targetNode = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                                                   where a.Attribute("Name").Value.Equals("COMAddin", StringComparison.InvariantCultureIgnoreCase)
                                                   select a).FirstOrDefault();

                            XElement propertyNode = (from a in targetNode.Element("Properties").Elements("Property")
                                                   where a.Attribute("Name").Value.Equals("Object", StringComparison.InvariantCultureIgnoreCase)
                                                   select a).FirstOrDefault();
                            propertyNode.Attribute("AnalyzeReturn").Value = "false";


                            targetNode = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                                          where a.Attribute("Name").Value.Equals("_CustomTaskPane", StringComparison.InvariantCultureIgnoreCase)
                                          select a).FirstOrDefault();

                            propertyNode = (from a in targetNode.Element("Properties").Elements("Property")
                                                     where a.Attribute("Name").Value.Equals("ContentControl", StringComparison.InvariantCultureIgnoreCase)
                                                     select a).FirstOrDefault();
                            propertyNode.Attribute("AnalyzeReturn").Value = "false";
                        }

                        if (dialog.EnableEarlyBind)
                        {
                            XElement projectNode = (from a in _comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project")
                                                    where a.Attribute("Name").Value.Equals("Office", StringComparison.InvariantCultureIgnoreCase)
                                                    select a).FirstOrDefault();

                            XElement targetNode = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                                                   where a.Attribute("Name").Value.Equals("IRibbonUI", StringComparison.InvariantCultureIgnoreCase)
                                                   select a).FirstOrDefault();

                            targetNode.Attribute("IsEarlyBind").Value = "true";

                            targetNode = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                                          where a.Attribute("Name").Value.Equals("IRibbonControl", StringComparison.InvariantCultureIgnoreCase)
                                          select a).FirstOrDefault();

                            targetNode.Attribute("IsEarlyBind").Value = "true";


                            targetNode = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                                          where a.Attribute("Name").Value.Equals("IRibbonExtensibility", StringComparison.InvariantCultureIgnoreCase)
                                          select a).FirstOrDefault();

                            targetNode.Attribute("IsEarlyBind").Value = "true";

                            targetNode = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                                          where a.Attribute("Name").Value.Equals("ICustomTaskPaneConsumer", StringComparison.InvariantCultureIgnoreCase)
                                          select a).FirstOrDefault();

                            targetNode.Attribute("IsEarlyBind").Value = "true";
                        }


                        if (dialog.EnableRangeType)
                        {
                            XElement projectNode = (from a in _comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project")
                                                    where a.Attribute("Name").Value.Equals("Excel", StringComparison.InvariantCultureIgnoreCase)
                                                    select a).FirstOrDefault();
                            if (null != projectNode)
                            { 
                                XElement targetNode = (from a in projectNode.Element("DispatchInterfaces").Elements("Interface")
                                                   where a.Attribute("Name").Value.Equals("Range", StringComparison.InvariantCultureIgnoreCase)
                                                   select a).FirstOrDefault();
                            

                                XElement templateNode = (from a in targetNode.Element("Properties").Elements("Property")
                                                       where a.Attribute("Name").Value.Equals("CurrentRegion", StringComparison.InvariantCultureIgnoreCase)
                                                       select a).FirstOrDefault();
                                templateNode = templateNode.Element("Parameters").Element("ReturnValue");


                                XElement propertyNode = (from a in targetNode.Element("Properties").Elements("Property")
                                                         where a.Attribute("Name").Value.Equals("_Default", StringComparison.InvariantCultureIgnoreCase)
                                                         select a).FirstOrDefault();
                                if (null != propertyNode)
                                { 
                                    foreach (XElement item in propertyNode.Elements("Parameters"))
                                    {
                                        XElement returnValue = item.Element("ReturnValue");
                                        foreach (XAttribute attrib in returnValue.Attributes())
                                            attrib.Value = templateNode.Attribute(attrib.Name.LocalName).Value;
                                    }

                                    propertyNode = (from a in targetNode.Element("Properties").Elements("Property")
                                                             where a.Attribute("Name").Value.Equals("Item", StringComparison.InvariantCultureIgnoreCase)
                                                             select a).FirstOrDefault();
                                }

                                foreach (XElement item in propertyNode.Elements("Parameters"))
                                {
                                    XElement returnValue = item.Element("ReturnValue");
                                    foreach (XAttribute attrib in returnValue.Attributes())
                                        attrib.Value = templateNode.Attribute(attrib.Name.LocalName).Value;
                                }
                            }
                        }

                        if (dialog.EnableSupportByVersion)
                        {
                            var libs = _comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document").Element("Libraries").Elements("Library");
                            foreach (XElement item in libs)
                            {
                                item.Attribute("Version").Value = GetVersionTag(item);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please load a Document first");
                }
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        private string GetVersionTag(XElement itemLibrary)
        {
            string description = itemLibrary.Attribute("Description").Value;
            string[] versions = new string[] { "14", "12", "11", "10", "9" };
            foreach (string item in versions)
            {
                if (description.IndexOf(item, 0, description.Length, StringComparison.InvariantCultureIgnoreCase) > -1)
                    return item;
            }

            if (description.IndexOf("OLE Automation", StringComparison.InvariantCultureIgnoreCase) > -1)
                return "2";

            if (description.IndexOf("2.5", StringComparison.InvariantCultureIgnoreCase) > -1)
                return "2.5";

            if (description.IndexOf("3.6", StringComparison.InvariantCultureIgnoreCase) > -1)
                return "3.6";

            if (description.IndexOf("5.3", StringComparison.InvariantCultureIgnoreCase) > -1)
                return "5.3";
             
            return itemLibrary.Attribute("Version").Value;
        }
     }
}