using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using LateBindingApi.CodeGenerator.Core;
 
namespace LateBindingApi.CodeGenerator.WFApplication
{
    public partial class FormMain : Form
    {
        #region Fields

        ItemFilter         _filter    = new ItemFilter();
        COMComponentReader _comReader = new COMComponentReader();

        #endregion

        #region Construction

        public FormMain()
        {
            InitializeComponent();
            this.Text = this.GetType().Assembly.GetName().Name;
        }

        #endregion

    
    }
}