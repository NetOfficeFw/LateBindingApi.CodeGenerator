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
        XElement _node;

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
            _node = node;
            sourceEditControl.Show(node);
            inheritedControl.Show(node);
            checkBoxCallQuit.Checked = Convert.ToBoolean(node.Attribute("AutomaticQuit").Value);
        }

        public void Clear()
        {
            sourceEditControl.Clear();
            inheritedControl.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            inheritedControl.Initialize(schema);
            _isInitialized = true;
        }

        #endregion

        private void checkBoxCallQuit_CheckedChanged(object sender, EventArgs e)
        {
            _node.Attribute("AutomaticQuit").Value = checkBoxCallQuit.Checked.ToString();
        }
    }
}
