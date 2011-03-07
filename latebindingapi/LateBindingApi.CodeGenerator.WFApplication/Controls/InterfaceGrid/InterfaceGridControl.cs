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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid
{
    /// <summary>
    /// shows interface details from document in grid
    /// </summary>
    public partial class InterfaceGridControl : UserControl
    { 
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire
        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion

        #region Construction

        public InterfaceGridControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods

        public void Show(XElement node)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("InterfaceGridControl is not initialized."));

            _showFlag = true;
            Clear();
            gridMethodsControl.Show(node.Element("Methods"));
            gridPropertiesControl.Show(node.Element("Properties"));
            sourceEditControl.Show(node);
            _showFlag = false;
        }

        public void Clear()
        {
            gridMethodsControl.Clear();
            gridPropertiesControl.Clear();
            sourceEditControl.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            gridMethodsControl.Initialize(schema);
            gridPropertiesControl.Initialize(schema);
            sourceEditControl.Initialize(schema);
            _isInitialized = true;
        }

        #endregion
    }
}
