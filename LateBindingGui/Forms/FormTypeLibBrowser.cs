using System;
using System.ComponentModel; 
using System.Collections.Generic;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.WFApplication
{
    public partial class FormTypeLibBrowser : Form
    {
        #region Construction

        public FormTypeLibBrowser()
        {
            InitializeComponent();
        }

        #endregion
        
        #region Properties
        
        public bool DoAsync
        {
            get
            {
                return checkBoxDoAsync.Checked;
            }
        }

        public bool AddToCurrentProject 
        {
            get 
            {
                return checkBoxAddToCurrentProject.Checked;  
            }
        }

        public string[] SelectedFiles
        {
            get
            {
                int columnsCount = typeLibBrowserControl1.ColumnsCount;
                string[] result = new string[typeLibBrowserControl1.SelectedItems.Count];
                for (int i = 0; i < typeLibBrowserControl1.SelectedItems.Count; i++)
                    result[i] = typeLibBrowserControl1.SelectedItems[i].SubItems[columnsCount-1].Text;

                return result;
            }
        }

        #endregion

        #region Button Trigger

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;  
            Close();
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        #endregion

        #region Control Trigger
        
        private void typeLibBrowserControl1_Click(object sender, EventArgs e)
        {
            buttonOkay.Enabled = (typeLibBrowserControl1.SelectedItems.Count > 0);
        }
        
        #endregion

        private void typeLibBrowserControl1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
