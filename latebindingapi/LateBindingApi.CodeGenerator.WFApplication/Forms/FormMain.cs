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
            enumGrid.Initialize(_comAnalyzer.Schema);
            classGrid.Initialize(_comAnalyzer.Schema);
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
            try
            {
                FormTypeLibBrowser formBrowser = new FormTypeLibBrowser();
                this.Refresh();
                if (DialogResult.OK == formBrowser.ShowDialog(this))
                    _comAnalyzer.LoadTypeLibraries(formBrowser.SelectedFiles, formBrowser.AddToCurrentProject, false);
            }
            catch (Exception throwedException)
            {
                FormShowError formError = new FormShowError(throwedException);
                formError.ShowDialog(this);
            }
            finally
            {
                statusStripMain.Items[0].Text = string.Empty;
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
        
        #endregion

        private void comAnalyzer_OnTypeLibrariesLoaded(TimeSpan timeElapsed)
        {
            try
            {
                //invisible all right panel content
                foreach (Control itemControl in splitContainerMain.Panel2.Controls)
                    itemControl.Visible = false;

                libraryTreeBrowser.Show(_comAnalyzer.Document.Element("LateBindingApi.CodeGenerator.Document"));
                menuItemSaveProject.Enabled = true;
                menuItemGenerateCode.Enabled = true;
                splitContainerMain.Visible = true;
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
                statusStripMain.Items[0].Text = message;
                statusStripMain.Refresh();
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
                        break;
                    case "Project":
                        projectGrid.Show(node);
                        projectGrid.Visible = true;
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
    }
}