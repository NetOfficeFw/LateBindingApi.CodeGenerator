using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.WFApplication
{
    partial class FormStatistics : Form
    {
        public FormStatistics(XDocument document)
        {
            InitializeComponent();
            string result = "";
            var projects = document.Element("LateBindingApi.CodeGenerator.Document").Element("Solution").Element("Projects").Elements("Project");
            foreach (var item in projects)
            {
                result += item.Attribute("Name") + Environment.NewLine;
                int coClassCount = item.Element("CoClasses").Elements("CoClass").Count();
                int dispatchCount = item.Element("DispatchInterfaces").Elements("Interface").Count();
                int interfaceCount = item.Element("Interfaces").Elements("Interface").Count();
                int enumCount = item.Element("Enums").Elements("Enum").Count();
                result += string.Format("Classes {0} Dispatch {1} Interface {2} Enums {3}{4}{4}", coClassCount, dispatchCount, interfaceCount, enumCount, Environment.NewLine);                  
            }
            textBoxMain.Text = result;
        }

        private void buttonOkay_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
