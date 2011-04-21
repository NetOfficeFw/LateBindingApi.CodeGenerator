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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.AliasGrid
{
    /// <summary>
    /// shows alias details from document in grid
    /// </summary>
    public partial class AliasGridControl : UserControl
    {
        #region Fields

        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion
        
        #region Construction

        public AliasGridControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods

        public void Show(XElement node)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("AliasGridControl is not initialized."));

            Clear();

            string name = node.Attribute("Name").Value;
            textBoxAlias.AppendText("Name:\t" + name + "\r\n");
            
            string intrinsic = node.Attribute("Intrinsic").Value;
            textBoxAlias.AppendText("Intrinsic:\t" + intrinsic + "\r\n\r\n");
            
            string version = GetDependencies(node.Element("RefLibraries"));
            textBoxAlias.AppendText("Versions:\t" + version + "\r\n\r\n");

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

                result += libNode.Attribute("Version").Value + "; ";
            }

            return result;
        }


        #endregion
    }
}
