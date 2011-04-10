using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

using LateBindingApi.CodeGenerator.ComponentAnalyzer;
 
namespace LateBindingApi.CodeGenerator.WFApplication
{
    public partial class FormMain : Form
    {
        #region Fields

        Analyzer _comAnalyzer = new Analyzer();

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
        }

       
        #endregion

        #region Menu Click Trigger

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

                    this.Cursor = Cursors.WaitCursor;
                    menuStripMain.Enabled = false; 
                    _comAnalyzer.LoadTypeLibraries(formBrowser.SelectedFiles, formBrowser.AddToCurrentProject, formBrowser.DoAsync);
                }
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        private void menuItemLoadProject_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "ProjectFiles|*.xml";
                if (DialogResult.OK == fileDialog.ShowDialog(this))
                {
                    DateTime startTime = DateTime.Now;
                    _comAnalyzer.LoadProject(fileDialog.FileName);
                    TimeSpan timeElapsed = DateTime.Now - startTime;
                    comAnalyzer_OnTypeLibrariesLoaded(timeElapsed);
                }
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        private void menuItemSaveProject_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "ProjectFiles|*.xml";
                if (DialogResult.OK == fileDialog.ShowDialog(this))
                {
                    _comAnalyzer.SaveProject(fileDialog.FileName);
                    MessageBox.Show("Project successfully saved.", this.GetType().Assembly.GetName().Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        private void menuItemGenerateCode_Click(object sender, EventArgs e)
        {
            try
            {
                FormGeneratorBrowser formBrowser = new FormGeneratorBrowser();
                if (DialogResult.OK == formBrowser.ShowDialog(this))
                {
                    ICodeGenerator generator = formBrowser.Selected;
                    if (DialogResult.OK == generator.ShowConfigDialog(this))
                        generator.Generate(_comAnalyzer.Document); 
                }
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
                menuStripMain.Enabled = true;
                menuItemSaveProject.Enabled = true;
                menuItemGenerateCode.Enabled = true;
                splitContainerMain.Visible = true;
                statusStripMain.Items[0].Text = string.Format("Loaded in {0}", timeElapsed);
                this.Cursor = Cursors.Default;
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
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
                // abort async operation
                if ((true == _comAnalyzer.DoAsync) && (true == _comAnalyzer.IsAlive))
                    _comAnalyzer.Abort();
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
        }

        #endregion
    }
}