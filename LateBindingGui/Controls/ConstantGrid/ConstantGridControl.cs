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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.ConstantGrid
{
    public partial class ConstantGridControl : UserControl
    {    
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire
        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion


        public ConstantGridControl()
        {
            InitializeComponent();
        }

        #region ControlMethods

        public void Show(XElement enumNode)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("ConstantGridControl is not initialized."));

            _showFlag = true;
            Clear();

            foreach (var item in enumNode.Descendants("Member"))
            {
                gridConstants.Rows.Add();
                DataGridViewRow newRow = gridConstants.Rows[gridConstants.Rows.Count - 1];
                newRow.Tag = item;

                newRow.Cells[0].Value = item.Attribute("Type").Value;
                newRow.Cells[0].ReadOnly = true;
                newRow.Cells[0].Style.BackColor = Color.DarkKhaki;

                newRow.Cells[1].Value = item.Attribute("Name").Value;
                newRow.Cells[1].Tag = item.Attribute("Name");

                newRow.Cells[2].Value = item.Attribute("Value").Value;
                newRow.Cells[2].Tag = item.Attribute("Value");

                newRow.Cells[3].Value = GetDependencies(item.Element("RefLibraries"));
                newRow.Cells[3].ReadOnly = true;
                newRow.Cells[3].Style.BackColor = Color.DarkKhaki;
            }

            _showFlag = false;
        }

        public void Clear()
        {
            gridConstants.Rows.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            gridConstants.Columns.Clear();
            gridConstants.Columns.Add("Type", "Type");
            gridConstants.Columns.Add("Name", "Name");
            gridConstants.Columns.Add("Value", "Value");
            gridConstants.Columns.Add("Versions", "Versions");
            gridConstants.Columns[0].ReadOnly = true;
            gridConstants.Columns[3].ReadOnly = true;

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
                DataGridViewCell selectedCell = gridConstants.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewColumn selectedColumn = gridConstants.Columns[e.ColumnIndex];
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
