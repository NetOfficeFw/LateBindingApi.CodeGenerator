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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.LibraryGrid
{
    /// <summary>
    /// shows library details from document in grid
    /// </summary>
    public partial class LibraryGridControl : UserControl
    {
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire
        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion

        #region Construction

        public LibraryGridControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods

        public void Show(XElement node)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("LibraryGridControl is not initialized."));

            _showFlag = true;

            Clear();
            foreach (var item in node.Elements("Library"))
            {
                dataGridLibraries.Rows.Add();
                DataGridViewRow newRow = dataGridLibraries.Rows[dataGridLibraries.Rows.Count - 1];
                
                foreach (var attribute in item.Attributes())
                {
                    string columnName =  attribute.Name.LocalName;
                    DataGridViewCell cell   = newRow.Cells[columnName];
                    cell.Value = attribute.Value;
                    cell.Tag = attribute;
                    cell.Style.BackColor = GetCellColor(columnName);
	            }
            }
            
            foreach (var item in node.Elements("Library"))
            {
                string libName = item.Attribute("Name").Value;
                string libDesc = item.Attribute("Description").Value;
                textBoxDepends.AppendText(libName + " - " + libDesc + Environment.NewLine);
                foreach (var depend in item.Elements("DependLib"))
                {
                    string dependName = depend.Attribute("Name").Value;
                    string dependDesc = depend.Attribute("Description").Value;
                    textBoxDepends.AppendText("\t" + dependName + " - " + dependDesc + Environment.NewLine);
                }
            }

            sourceControl.Show(node);
            _showFlag = false;
        }

        public void Clear()
        {
            textBoxDepends.Clear();
            dataGridLibraries.Rows.Clear();
            sourceControl.Clear(); 
        }

        public void Initialize(XmlSchema schema)
        {
            dataGridLibraries.Columns.Clear();

            XmlQualifiedName qname = new XmlQualifiedName("Library", "http://latebindingapi.codeplex.com/XMLSchema.xsd");
            XmlSchemaComplexType type = (XmlSchemaComplexType)schema.SchemaTypes[qname];
            foreach (XmlSchemaAttribute item in type.Attributes)
            {
                dataGridLibraries.Columns.Add(item.Name, item.Name);
                DataGridViewColumn newColumn = dataGridLibraries.Columns[dataGridLibraries.Columns.Count - 1];
                newColumn.ReadOnly = true;
                newColumn.Width = 50;

                if(newColumn.Name == "Version")
                    newColumn.ReadOnly = false;

                if (newColumn.Name == "Description")
                    newColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill; 

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
            DataGridViewColumn column = dataGridLibraries.Columns[columnName];
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
                DataGridViewCell selectedCell = dataGridLibraries.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewColumn selectedColumn = dataGridLibraries.Columns[e.ColumnIndex];
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
