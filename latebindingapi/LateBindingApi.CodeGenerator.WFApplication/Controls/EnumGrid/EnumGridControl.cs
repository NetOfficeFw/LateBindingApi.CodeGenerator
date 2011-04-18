using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using System.Text;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.EnumGrid
{
    /// <summary>
    /// shows enum details from document in grid
    /// </summary>
    public partial class EnumGridControl : UserControl
    {
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire
        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion

        #region Construction

        public EnumGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region ControlMethods

        public void Show(XElement enumNode)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("EnumGridControl is not initialized."));

            _showFlag = true;
            Clear();
            
            textBoxKey.Text = enumNode.Attribute("Key").Value;

            foreach (var item in enumNode.Descendants("Member"))
            {
                gridMembers.Rows.Add();
                DataGridViewRow newRow = gridMembers.Rows[gridMembers.Rows.Count - 1];
                newRow.Tag = item;
                newRow.Cells[0].Value = item.Attribute("Name").Value;
                newRow.Cells[0].Tag = item.Attribute("Name");
                newRow.Cells[1].Value = item.Attribute("Value").Value;
                newRow.Cells[1].Tag = item.Attribute("Value");
                newRow.Cells[2].Value = GetDependencies(item.Element("RefLibraries"));
                newRow.Cells[2].ReadOnly = true;
                newRow.Cells[2].Style.BackColor = Color.DarkKhaki; 
            }

            _showFlag = false;
        }

        public void Clear()
        {
            gridMembers.Rows.Clear();
            textBoxKey.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            gridMembers.Columns.Clear();
            gridMembers.Columns.Add("Name", "Name");
            gridMembers.Columns.Add("Value", "Value");
            gridMembers.Columns.Add("Versions", "Versions");
            gridMembers.Columns[2].ReadOnly = true;

            _isInitialized = true;
        }

        #endregion

        #region Private Methods

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
        
        #region Trigger

        private void gridMembers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // change writable attributes
                if ((e.RowIndex < 0) || (true == _showFlag)) return;
                DataGridViewCell selectedCell = gridMembers.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewColumn selectedColumn = gridMembers.Columns[e.ColumnIndex];
                if (false == selectedColumn.ReadOnly)
                {
                    XAttribute attribute = selectedCell.Tag as XAttribute;
                    attribute.Value = selectedCell.Value as string;
                }
            }
            catch (Exception throwedException)
            {
                string message = string.Format("An error occured.{0}Details:{0}{1}", Environment.NewLine, throwedException.Message);
                MessageBox.Show(this, message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
