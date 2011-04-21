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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.ModulGrid
{
    /// <summary>
    /// shows module details from document in grid
    /// </summary>
    public partial class ModulGridControl : UserControl
    {
        #region Fields

        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion
        
        #region Construction

        public ModulGridControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods

        public void Show(XElement node)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("ModulGridControl is not initialized."));

            Clear();
            gridMethodsControl.Show(node.Element("Methods"));
            sourceEditControl.Show(node);
        }

        public void Clear()
        {
            gridMethodsControl.Clear();
            sourceEditControl.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            gridMethodsControl.Initialize(schema);
            _isInitialized = true;
        }

        #endregion
    }
}
