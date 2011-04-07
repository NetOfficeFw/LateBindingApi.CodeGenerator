using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.CSharp
{
    partial class FormConfigDialog : Form
    {
        #region Construction

        public FormConfigDialog()
        {
            InitializeComponent();
            textBoxFolder.Text = Application.StartupPath;
        }
        
        #endregion

        #region Properties

        public Settings Selected
        {
            get 
            {
                Settings newSettings = new Settings();
                newSettings.Folder = textBoxFolder.Text.Trim();
                return newSettings;
            }
        }

        #endregion

        #region Trigger

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; 
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (DialogResult.OK == folderDialog.ShowDialog(this))
                textBoxFolder.Text = folderDialog.SelectedPath;
        }

        #endregion
    }
}
