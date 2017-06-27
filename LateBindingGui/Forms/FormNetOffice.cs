using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.WFApplication
{
    partial class FormNetOffice : Form
    {
        public FormNetOffice()
        {
            InitializeComponent();
        }

        public bool EnableSolutionName
        {
            get
            {
                return checkBoxSolutionName.Checked;
            }
        }

        public string SolutionName
        {
            get
            {
                return textBoxSolutionName.Text.Trim();
            }
        }


        public bool EnableAssemblyVersion
        {
            get 
            {
                return checkBoxAssemblyVersion.Checked;
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return textBoxAssemblyVersion.Text.Trim();
            }
        }

        public bool EnableNamespaces
        {
            get
            {
                return  checkBoxNamespaces.Checked;
            }
        }

        public bool EnableCallQuit
        {
            get
            {

                return checkBoxCallQuit.Checked;
            }
        }

        public bool EnableAnalyzeReturn
        {
            get
            {
                return checkBoxAnalyzeReturn.Checked;
            }
        }

        public bool EnableEarlyBind
        {
            get
            {
                return checkBoxEarlyBind.Checked;
            }
        }
         
        public bool EnableRangeType
        {
            get
            {
                return checkBoxRangeType.Checked;
            }
        }

        public bool EnableSupportByVersion
        {
            get
            {
                return checkBoxSupportByVersion.Checked;
            }
        }


        private void checkBoxAssemblyVersion_CheckedChanged(object sender, EventArgs e)
        {
            textBoxAssemblyVersion.Enabled = checkBoxAssemblyVersion.Checked;
        }

        private void checkBoxSolutionName_CheckedChanged(object sender, EventArgs e)
        {
            textBoxSolutionName.Enabled = checkBoxSolutionName.Checked;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
      
    }
}
