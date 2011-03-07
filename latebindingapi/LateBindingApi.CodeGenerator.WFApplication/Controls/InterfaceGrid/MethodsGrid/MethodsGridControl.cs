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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.MethodsGrid
{
    public partial class MethodsGridControl : UserControl
    {
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire
        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion

        #region Construction

        public MethodsGridControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods

        public void Show(XElement methodsNode)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("MethodsGridControl is not initialized."));

            if (methodsNode.Name.LocalName != "Methods")
                throw (new NotSupportedException("node is not a Methods node."));

            _showFlag = true;

            Clear();
            foreach (var item in methodsNode.Elements("Method"))
            {
                foreach (var itemParameters in item.Elements("Parameters"))
                {
                    // add new row
                    gridMethods.Rows.Add();
                    DataGridViewRow newRow = gridMethods.Rows[gridMethods.Rows.Count - 1];
                    newRow.Tag = item;

                    //
                    foreach (var attribute in item.Attributes())
                    {
                        string columnName = attribute.Name.LocalName;
                        DataGridViewCell cell = newRow.Cells[columnName];
                        cell.Value = attribute.Value;       
                        cell.Style.BackColor = GetCellColor(columnName);
                        cell.Tag = attribute;
                    }
                    
                    newRow.Cells["ReturnType"].Value = itemParameters.Element("ReturnValue").Attribute("Type").Value;
                    newRow.Cells["Versions"].Value = "ABC";

                    newRow.Cells["ReturnType"].Style.BackColor = GetCellColor("ReturnType");
                    newRow.Cells["Versions"].Style.BackColor = GetCellColor("Versions");
                    newRow.Cells["Delete"].Style.BackColor = Color.FromKnownColor(KnownColor.Gray);
                    newRow.Cells["Edit"].Style.BackColor = Color.FromKnownColor(KnownColor.Gray);
                    
                }
            }

            _showFlag = false;
        }

        public void Clear()
        {
            gridMethods.Rows.Clear();
            parametersGridControl.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            gridMethods.Columns.Clear();

            gridMethods.Columns.Add("ReturnType", "ReturnType");
            DataGridViewColumn newColumnReturnType = gridMethods.Columns[gridMethods.Columns.Count - 1];
            newColumnReturnType.ReadOnly = true;
            newColumnReturnType.Width = 120;
            
            XmlQualifiedName qname = new XmlQualifiedName("Method", "http://latebindingapi.codeplex.com/XMLSchema.xsd");
            XmlSchemaComplexType type = (XmlSchemaComplexType)schema.SchemaTypes[qname];
            foreach (XmlSchemaAttribute item in type.Attributes)
            {
                gridMethods.Columns.Add(item.Name, item.Name);
                DataGridViewColumn newColumn = gridMethods.Columns[gridMethods.Columns.Count - 1];
                newColumn.ReadOnly = true;
                newColumn.Width = 50;
            }

            gridMethods.Columns.Add("Versions", "Versions");
            DataGridViewColumn newColumnVersions = gridMethods.Columns[gridMethods.Columns.Count - 1];
            newColumnVersions.ReadOnly = true;
            newColumnVersions.Width = 50;

            DataGridViewButtonColumn newButtonColumn = new DataGridViewButtonColumn();
            gridMethods.Columns.Add(newButtonColumn);
            newButtonColumn.Name = "Delete";
            newButtonColumn.HeaderText = "Delete";
            newButtonColumn.ReadOnly = true;
            newButtonColumn.Width = 70;
            
            newButtonColumn = new DataGridViewButtonColumn();
            gridMethods.Columns.Add(newButtonColumn);
            newButtonColumn.Name = "Edit";
            newButtonColumn.HeaderText = "Edit";
            newButtonColumn.ReadOnly = true;
            newButtonColumn.Width = 70;

            parametersGridControl.Initialize(schema);

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
            DataGridViewColumn column = gridMethods.Columns[columnName];
            if (true == column.ReadOnly)
                return Color.DarkKhaki;
            else
                return Color.White;
        }

        #endregion

        #region Trigger

        private void gridMethods_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((e.RowIndex < 0) || (true == _showFlag) || (false == _isInitialized)) 
                    return;

                // change writable attributes
                DataGridViewCell selectedCell = gridMethods.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewColumn selectedColumn = gridMethods.Columns[e.ColumnIndex];
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

        private void gridMethods_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (true == _showFlag) || (false == _isInitialized)) 
                return;

            DataGridViewRow selectRow = gridMethods.Rows[e.RowIndex];
            if (gridMethods.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                string typeAction = gridMethods.Columns[e.ColumnIndex].HeaderText;
                switch (typeAction)
                {
                    case "Edit":
                    {
                        MessageBox.Show("Edit"); 
                        break;
                    }
                    case "Delete":
                    {
                        string question = string.Format("Delete {0} ?", selectRow.Cells[1].Value);
                        DialogResult dr = MessageBox.Show(question, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            XElement node = selectRow.Tag as XElement; 
                            node.Remove();                            
                            gridMethods.Rows.Remove(selectRow);
                        }
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
