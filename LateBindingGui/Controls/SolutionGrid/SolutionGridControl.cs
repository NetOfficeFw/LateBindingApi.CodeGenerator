using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Text;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.SolutionGrid
{
    public partial class SolutionGridControl : UserControl
    {
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire
        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion

        #region Construction

        public SolutionGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region ControlMethods

        public void Show(XElement solutionNode)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("SolutionGridControl is not initialized."));
            
            _showFlag = true;

            Clear();

            gridSolution.Rows.Add();
            DataGridViewRow newRow = gridSolution.Rows[gridSolution.Rows.Count - 1];
            newRow.Tag = solutionNode;

            newRow.Cells["Name"].Value = "Solution";
            newRow.Cells["Name"].Style.BackColor = Color.DarkKhaki;
            newRow.Cells["Value"].Value = solutionNode.Attribute("Name").Value;
            newRow.Cells["Value"].Tag = solutionNode.Attribute("Name");
            _showFlag = false;
        }

        public void Clear()
        {
            gridSolution.Rows.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            gridSolution.Columns.Clear();
            gridSolution.Columns.Add("Name", "Name");
            gridSolution.Columns["Name"].ReadOnly = true;
            gridSolution.Columns.Add("Value", "Value");
            gridSolution.Columns["Value"].Width = 200;
            _isInitialized = true;
        }

        #endregion

        #region Trigger

        private void gridSolution_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // change writable attributes
                if ((e.RowIndex < 0) || (true == _showFlag)) return;
                DataGridViewCell selectedCell = gridSolution.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewColumn selectedColumn = gridSolution.Columns[e.ColumnIndex];
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
