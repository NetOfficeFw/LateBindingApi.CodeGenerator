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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.ClassGrid
{
    public partial class ClassGridControl : UserControl
    {
        #region Fields

        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion

        #region Construction

        public ClassGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region ControlMethods

        public void Show(XElement node)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("ClassGridControl is not initialized."));

            Clear();

            sourceEditControl.Show(node);
        }

        public void Clear()
        {
            gridMethodsControl.Clear();
            gridPropertiesControl.Clear();
            sourceEditControl.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            _isInitialized = true;
        }

        #endregion
    }
}
