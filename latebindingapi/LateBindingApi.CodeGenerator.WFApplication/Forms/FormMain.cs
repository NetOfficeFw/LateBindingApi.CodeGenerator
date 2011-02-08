using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

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
        }

       
        #endregion

        #region Gui Trigger

        void comAnalyzer_Update(object sender, string message)
        {
            statusStripMain.Items[0].Text = message;
            statusStripMain.Refresh();
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
        
        #endregion

        private void menuItemLoadTypeLibrary_Click(object sender, EventArgs e)
        {
            try
            {
                FormTypeLibBrowser formBrowser = new FormTypeLibBrowser();
                if (DialogResult.OK == formBrowser.ShowDialog(this))
                {
                    _comAnalyzer.LoadCOMComponents(formBrowser.SelectedFiles, formBrowser.AddToCurrentProject);
                    OnTypeLibrariesLoaded();
                }
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
                    _comAnalyzer.LoadProject(fileDialog.FileName);
                    OnTypeLibrariesLoaded();
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

        private void OnTypeLibrariesLoaded()
        {
            menuItemSaveProject.Enabled = true;
            menuItemGenerateCode.Enabled = true;
        }
    }
}