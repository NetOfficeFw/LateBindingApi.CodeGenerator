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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.InterfaceGrid.PropertiesGrid
{
    public partial class PropertiesGridControl : UserControl
    {
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire
        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion

        #region Construction

        public PropertiesGridControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods

        public void Show(XElement methodsNode)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("PropertiesGridControl is not initialized."));

            if (methodsNode.Name.LocalName != "Properties")
                throw (new NotSupportedException("node is not a Properties node."));

            gridProperties.Tag = methodsNode;

            _showFlag = true;

            Clear();
            foreach (var item in methodsNode.Elements("Property"))
            {
                if (false == FilterPassed(item))
                    continue;

                foreach (var itemParameters in item.Elements("Parameters"))
                {
                    // add new row
                    gridProperties.Rows.Add();
                    DataGridViewRow newRow = gridProperties.Rows[gridProperties.Rows.Count - 1];
                    newRow.Tag = itemParameters;

                    foreach (var attribute in item.Attributes())
                    {
                        string columnName = attribute.Name.LocalName;
                        if ("Underlying" != columnName)
                        { 
                            DataGridViewCell cell = newRow.Cells[columnName];
                            cell.Value = attribute.Value;       
                            cell.Style.BackColor = GetCellColor(columnName);
                            cell.Tag = attribute;
                        }
                    }
                    
                    newRow.Cells["ReturnType"].Value = itemParameters.Element("ReturnValue").Attribute("Type").Value;
                    newRow.Cells["Versions"].Value = GetDependencies(itemParameters.Element("RefLibraries"));
                    
                    newRow.Cells["ReturnType"].Style.BackColor = GetCellColor("ReturnType");
                    newRow.Cells["Versions"].Style.BackColor = GetCellColor("Versions");
                    newRow.Cells["Delete"].Style.BackColor = Color.FromKnownColor(KnownColor.Gray);
           
                }
            }

            _showFlag = false;
        }

        public void Clear()
        {
            gridProperties.Rows.Clear();
            parametersGridControl.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            gridProperties.Columns.Clear();

            gridProperties.Columns.Add("ReturnType", "ReturnType");
            DataGridViewColumn newColumnReturnType = gridProperties.Columns[gridProperties.Columns.Count - 1];
            newColumnReturnType.ReadOnly = true;
            newColumnReturnType.Width = 120;
            
            XmlQualifiedName qname = new XmlQualifiedName("Method", "http://latebindingapi.codeplex.com/XMLSchema.xsd");
            XmlSchemaComplexType type = (XmlSchemaComplexType)schema.SchemaTypes[qname];
            foreach (XmlSchemaAttribute item in type.Attributes)
            {
                gridProperties.Columns.Add(item.Name, item.Name);
                DataGridViewColumn newColumn = gridProperties.Columns[gridProperties.Columns.Count - 1];
                newColumn.ReadOnly = true;
                newColumn.Width = 150;
            }

            gridProperties.Columns.Add("InvokeKind", "InvokeKind");
            DataGridViewColumn newColumnVersions = gridProperties.Columns[gridProperties.Columns.Count - 1];
            newColumnVersions.ReadOnly = true;
            newColumnVersions.Width = 150;

            gridProperties.Columns.Add("Versions", "Versions");
            newColumnVersions = gridProperties.Columns[gridProperties.Columns.Count - 1];
            newColumnVersions.ReadOnly = true;
            newColumnVersions.Width = 150;

            DataGridViewButtonColumn newButtonColumn = new DataGridViewButtonColumn();
            gridProperties.Columns.Add(newButtonColumn);
            newButtonColumn.Name = "Delete";
            newButtonColumn.HeaderText = "Delete";
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
            DataGridViewColumn column = gridProperties.Columns[columnName];
            if (true == column.ReadOnly)
                return Color.DarkKhaki;
            else
                return Color.White;
        }
        
        /// <summary>
        /// returns supported library versions of an element as string 
        /// </summary>
        /// <param name="refLibraries"></param>
        /// <returns></returns>
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

        /// <summary>
        /// returns method name matched with filter
        /// </summary>
        /// <param name="methodNode"></param>
        /// <returns></returns>
        private bool FilterPassed(XElement methodNode)
        {
            string searchText = textBoxMethodFilter.Text.Trim();
            if ("" == searchText)
                return true;

            string caption = methodNode.Attribute("Name").Value;
            if (caption.IndexOf(searchText, StringComparison.InvariantCultureIgnoreCase) > -1)
                return true;
            else
                return false;
        }

        #endregion

        #region Trigger

        private void gridProperties_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if ((e.RowIndex < 0) || (true == _showFlag) || (false == _isInitialized)) 
                    return;

                // change writable attributes
                DataGridViewCell selectedCell = gridProperties.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewColumn selectedColumn = gridProperties.Columns[e.ColumnIndex];
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

        private void gridProperties_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.RowIndex < 0) || (true == _showFlag) || (false == _isInitialized)) 
                return;

            DataGridViewRow selectRow = gridProperties.Rows[e.RowIndex];
            if (gridProperties.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                string typeAction = gridProperties.Columns[e.ColumnIndex].HeaderText;
                switch (typeAction)
                {
                    case "Delete":
                    {
                        string question = string.Format("Delete {0} ?", selectRow.Cells[1].Value);
                        DialogResult dr = MessageBox.Show(question, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            XElement node = selectRow.Tag as XElement;
                            XElement parentNode = node.Parent;
                            node.Remove();

                            if (parentNode.Elements("Parameters").Count() == 0)
                                parentNode.Remove();

                            gridProperties.Rows.Remove(selectRow);
                         }
                         break;
                    }
                }
            }
            else
            {
                XElement node = selectRow.Tag as XElement;
                parametersGridControl.Show(node);
            }
        }
        
        private void textBoxMethodFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return)
            {
                XElement methodsNode = gridProperties.Tag as XElement;
                Show(methodsNode);
            }
        }

        #endregion
    }
}
