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


namespace LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.ParametersGrid
{ 
    /// <summary>
    /// shows parameters details from document in grid
    /// </summary>
    public partial class ParametersGridControl : UserControl
    {  
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire
        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion

        #region Construction
        
        public ParametersGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region ControlMethods

        /// <summary>
        /// shows alle parameter in node
        /// </summary>
        /// <param name="node">node type Parameters</param>
        public void Show(XElement node)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("ParametersGridControl is not initialized."));

            if(node.Name.LocalName != "Parameters")
                throw (new NotSupportedException("node is not a Parameters node."));

            _showFlag = true;
            
            Clear();
            foreach (var item in node.Elements("Parameter"))
            {
                gridParameters.Rows.Add();
                DataGridViewRow newRow = gridParameters.Rows[gridParameters.Rows.Count - 1];

                foreach (var attribute in item.Attributes())
                {
                    string columnName = attribute.Name.LocalName;
                    DataGridViewCell cell = newRow.Cells[columnName];
                    cell.Value = attribute.Value;
                    cell.Tag = attribute;
                    cell.Style.BackColor = GetCellColor(columnName);
                }
            }

            _showFlag = false;
        }

        public void Clear()
        {
            gridParameters.Rows.Clear();
        }

        /// <summary>
        /// initialize grid columns with type 'parameter'
        /// </summary>
        /// <param name="schema"></param>
        public void Initialize(XmlSchema schema)
        {
            gridParameters.Columns.Clear();

            XmlQualifiedName qname = new XmlQualifiedName("Parameter", "http://latebindingapi.codeplex.com/XMLSchema.xsd");
            XmlSchemaComplexType type = (XmlSchemaComplexType)schema.SchemaTypes[qname];
            foreach (XmlSchemaAttribute item in type.Attributes)
            {
                gridParameters.Columns.Add(item.Name, item.Name);
                DataGridViewColumn newColumn = gridParameters.Columns[gridParameters.Columns.Count - 1];
                newColumn.ReadOnly = true;
                newColumn.Width = 50;
            }

            DataGridViewButtonColumn newButtonColumn = new DataGridViewButtonColumn();
            gridParameters.Columns.Add(newButtonColumn);
            newButtonColumn.HeaderText = "Delete";
            newButtonColumn.ReadOnly = true;
            newButtonColumn.Width = 50;

            newButtonColumn = new DataGridViewButtonColumn();
            gridParameters.Columns.Add(newButtonColumn);
            newButtonColumn.HeaderText = "Edit";
            newButtonColumn.ReadOnly = true;
            newButtonColumn.Width = 50;

            _isInitialized = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// returns readonly or writable color for cell
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private Color GetCellColor(string columnName)
        {
            DataGridViewColumn column = gridParameters.Columns[columnName];
            if (true == column.ReadOnly)
                return Color.DarkKhaki;
            else
                return Color.White;
        }

        #endregion

        #region Trigger

        private void dataGridLibraries_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // change writable attributes
                if ((e.RowIndex < 0) || (true == _showFlag)) return;
                DataGridViewCell selectedCell = gridParameters.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewColumn selectedColumn = gridParameters.Columns[e.ColumnIndex];
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
