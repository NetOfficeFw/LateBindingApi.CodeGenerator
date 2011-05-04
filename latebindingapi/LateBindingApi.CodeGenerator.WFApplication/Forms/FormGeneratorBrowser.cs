using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using LateBindingApi.CodeGenerator.ComponentAnalyzer;

namespace LateBindingApi.CodeGenerator.WFApplication
{
    partial class FormGeneratorBrowser : Form
    {
        #region Reflection ICodeGenerator

        internal static List<ICodeGenerator> _codeGeneratorList;

        internal static void CollectAvailableCodeGenerators()
        {
            string[] assemblies = System.IO.Directory.GetFiles(Application.StartupPath, "*.dll");
            foreach (var itemPath in assemblies)
            {
                string assemblyName = System.IO.Path.GetFileName(itemPath);
                if (true == assemblyName.StartsWith("LateBindingApi"))
                {
                    Assembly assembly = Assembly.LoadFile(itemPath);
                    foreach (Type itemType in assembly.GetExportedTypes())
                    {
                        Type interfaceType = itemType.GetInterface("LateBindingApi.CodeGenerator.ComponentAnalyzer.ICodeGenerator");
                        if (null != interfaceType)
                        {
                            ICodeGenerator component = Activator.CreateInstance(itemType) as ICodeGenerator;
                            _codeGeneratorList.Add(component);
                        }
                    }
                }
            }
        }

        public static ICodeGenerator[] CodeGenerators
        {
            get
            {
                if (null == _codeGeneratorList)
                {
                    _codeGeneratorList = new List<ICodeGenerator>();
                    CollectAvailableCodeGenerators();
                }
                return _codeGeneratorList.ToArray();
            }
        }

        #endregion

        #region Properties

        public ICodeGenerator Selected
        {
            get 
            {
                return listViewGenerators.SelectedItems[0].Tag as ICodeGenerator;
            }
        }

        #endregion

        #region Construction

        public FormGeneratorBrowser()
        {
            InitializeComponent();

            int i = 1;
            foreach (ICodeGenerator item in CodeGenerators)
            {
                ListViewItem newRow = listViewGenerators.Items.Add(i.ToString());
                newRow.SubItems.Add(item.Name);
                newRow.SubItems.Add(item.Version.ToString());
                newRow.SubItems.Add(item.Description);
                newRow.Tag = item; 
                i++;
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

        private void listViewGenerators_Click(object sender, EventArgs e)
        { 
            buttonOkay.Enabled = (listViewGenerators.SelectedItems.Count > 0);
        }

        private void listViewGenerators_DoubleClick(object sender, EventArgs e)
        {
            if (listViewGenerators.SelectedItems.Count > 0)
                buttonOkay_Click(this, new EventArgs());
        }

        #endregion

        private void FormGeneratorBrowser_KeyDown(object sender, KeyEventArgs e)
        {
            // alt and number marks and double click the ListViewItem
            if ((e.Modifiers == Keys.Alt) && ((e.KeyValue >= 49) && (e.KeyValue <= 57)))
            {
                int itemNumber = e.KeyValue - 48;
                if (itemNumber <= listViewGenerators.Items.Count)
                {
                    listViewGenerators.Items[itemNumber - 1].Selected = true;
                    listViewGenerators_DoubleClick(this, new EventArgs());
                }
            }
        }
    }
}
