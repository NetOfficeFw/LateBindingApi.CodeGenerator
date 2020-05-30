using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.Documentation
{
    partial class FormConfigDialog : Form
    {
        #region Construction

        public FormConfigDialog()
        {
            InitializeComponent();

        }
        
        #endregion

        #region Properties

        public Settings Selected
        {
            get
            {
               Settings settings = new Settings();
               settings.Folder = textBoxFolder.Text.Trim();
               return settings;
            }
        }
        #endregion

        #region Trigger

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (DialogResult.OK == folderDialog.ShowDialog(this))
                textBoxFolder.Text = folderDialog.SelectedPath;
        }

        private void textBoxFolder_TextChanged(object sender, EventArgs e)
        {
            buttonOk.Enabled = (textBoxFolder.Text.Trim() != "");
        }

        #endregion
    }
}
