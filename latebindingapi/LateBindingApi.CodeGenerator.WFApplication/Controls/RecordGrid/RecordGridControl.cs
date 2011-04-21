using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.RecordGrid
{    
    /// <summary>
    /// shows record details from document in grid
    /// </summary>
    public partial class RecordGridControl : UserControl
    {
      
        #region Fields

        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion
        
        #region Construction

        public RecordGridControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods
        
        public void Show(XElement node)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("RecordGridControl is not initialized."));

            Clear();

            string version = "SupportByLibrary[" + GetDependencies(node.Element("RefLibraries")) + "]\r\n";
            textBoxAlias.AppendText(version);
            
            string name = "struct " + node.Attribute("Name").Value + "\r\n{\r\n";
            textBoxAlias.AppendText(name);

            foreach (XElement itemMember in node.Element("Members").Elements("Member"))
            {
                string arr = "";
                if ("true" == itemMember.Attribute("IsArray").Value)
                    arr = "[]";

                string member = "\tSupportByLibrary[" + GetDependencies(node.Element("RefLibraries")) + "]\r\n";
                member += "\t" + itemMember.Attribute("Type").Value + arr + " " + itemMember.Attribute("Name").Value + ";\r\n";
                textBoxAlias.AppendText(member);
            }
            textBoxAlias.AppendText("}\r\n");
         
            sourceEditControl.Show(node);
        }

        public void Clear()
        {
            textBoxAlias.Clear();
            sourceEditControl.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            _isInitialized = true;
        }

        #endregion

        #region Methods

        private string GetDependencies(XElement refLibraries)
        {
            string result = "";
            XElement librariesNode = refLibraries.Document.Descendants("Libraries").FirstOrDefault();

            foreach (var item in refLibraries.Descendants("Ref"))
            {
                string refKey = item.Attribute("Key").Value;

                var libNode = (from a in librariesNode.Elements()
                               where a.Attribute("Key").Value.Equals(refKey, StringComparison.InvariantCultureIgnoreCase)
                               select a).FirstOrDefault();

                result += libNode.Attribute("Version").Value + ",";
            }

            if ("," == result.Substring(result.Length - 1))
                result = result.Substring(0, result.Length - 1);

            return result;
        }


        #endregion
    }
}
