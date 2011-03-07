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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.ProjectGrid
{    
    /// <summary>
    /// shows project details from document in grid
    /// </summary>
    public partial class ProjectGridControl : UserControl
    {  
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire
        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion

        #region Construction
        
        public ProjectGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region ControlMethods

        public void Show(XElement node)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("ProjectGridControl is not initialized."));

            _showFlag = true;

            Clear(); 

            dataGridProjects.Rows.Add();
            DataGridViewRow newRow = dataGridProjects.Rows[dataGridProjects.Rows.Count - 1];

            foreach (var attribute in node.Attributes())
            {
                string columnName = attribute.Name.LocalName;
                DataGridViewCell cell = newRow.Cells[columnName];
                cell.Value = attribute.Value;
                cell.Tag = attribute;
                cell.Style.BackColor = GetCellColor(columnName);
            }

            _showFlag = false;
        }

        public void Clear()
        {
            dataGridProjects.Rows.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            dataGridProjects.Columns.Clear();

            XmlQualifiedName qname = new XmlQualifiedName("Project", "http://latebindingapi.codeplex.com/XMLSchema.xsd");
            XmlSchemaComplexType type = (XmlSchemaComplexType)schema.SchemaTypes[qname];
            foreach (XmlSchemaAttribute item in type.Attributes)
            {
                dataGridProjects.Columns.Add(item.Name, item.Name);
                DataGridViewColumn newColumn = dataGridProjects.Columns[dataGridProjects.Columns.Count - 1];
                newColumn.ReadOnly = true;
                newColumn.Width = 100;

                if (newColumn.Name == "Namespace")
                {
                    newColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    newColumn.ReadOnly = false;
                }
            }
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
            DataGridViewColumn column = dataGridProjects.Columns[columnName];
            if (true == column.ReadOnly)
                return Color.DarkKhaki;
            else
                return Color.White;
        }

        #endregion

        #region Trigger

        private void dataGridProjects_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // change writable attributes
                if ((e.RowIndex < 0) || (true == _showFlag)) return;
                DataGridViewCell selectedCell = dataGridProjects.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewColumn selectedColumn = dataGridProjects.Columns[e.ColumnIndex];
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
