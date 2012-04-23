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

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.InheritedGrid
{
    public partial class InheritedGridControl : UserControl
    {     
        #region Fields

        bool _isInitialized;    // stores control was initalized with Initialize() method

        #endregion
       
        #region Construction
        
        public InheritedGridControl()
        {
            InitializeComponent();
        }

        #endregion
 
        #region ControlMethods

        public void Show(XElement node)
        {
            if (!_isInitialized)
                throw (new NotSupportedException("InheritedGridControl is not initialized."));
            
            Clear();

            gridInherited.Tag = node.Document.FirstNode;

            XElement inheritedNode = node.Element("Inherited");
            if (null != inheritedNode)
            { 
                foreach (var item in inheritedNode.Elements("Ref"))
                {
                    string key = item.Attribute("Key").Value;
                    gridInherited.Rows.Add();
                    DataGridViewRow newRow = gridInherited.Rows[gridInherited.Rows.Count - 1];
                    newRow.Cells["Key"].Value = key;
                    newRow.Cells["Name"].Value = GetInterfaceNode(key).Attribute("Name").Value;
                    newRow.Cells["Versions"].Value = GetDependencies(item.Element("RefLibraries"));
                }
            }

            XElement defaultNode = node.Element("DefaultInterfaces");
            if (null != defaultNode)
            { 
                foreach (var item in defaultNode.Elements("Ref"))
                {
                    string key = item.Attribute("Key").Value;
                    gridDefault.Rows.Add();
                    DataGridViewRow newRow = gridDefault.Rows[gridDefault.Rows.Count - 1];
                    newRow.Cells["Key"].Value = key;
                    newRow.Cells["Name"].Value = GetInterfaceNode(key).Attribute("Name").Value;
                    newRow.Cells["Versions"].Value = GetDependencies(item.Element("RefLibraries"));
                }
            }

            XElement eventNode = node.Element("EventInterfaces");
            if (null != defaultNode)
            { 
                foreach (var item in eventNode.Elements("Ref"))
                {
                    string key = item.Attribute("Key").Value;
                    gridEvent.Rows.Add();
                    DataGridViewRow newRow = gridEvent.Rows[gridEvent.Rows.Count - 1];
                    newRow.Cells["Key"].Value = key;
                    newRow.Cells["Name"].Value = GetInterfaceNode(key).Attribute("Name").Value;
                    newRow.Cells["Versions"].Value = GetDependencies(item.Element("RefLibraries"));
                }
            }
        }

        public void Clear()
        {
            gridDefault.Rows.Clear();
            gridInherited.Rows.Clear();
            gridEvent.Rows.Clear();
        }

        public void Initialize(XmlSchema schema)
        {
            SetupGrid(gridDefault);
            SetupGrid(gridInherited);
            SetupGrid(gridEvent);
 
            _isInitialized = true;
        }

        private void SetupGrid(DataGridView grid)
        {
            grid.Columns.Clear();

            grid.Columns.Add("Name", "Name");
            DataGridViewColumn newColumn = grid.Columns[grid.Columns.Count - 1];
            newColumn.ReadOnly = true;
            newColumn.Width = 150;

            grid.Columns.Add("Versions", "Versions");
            newColumn = grid.Columns[grid.Columns.Count - 1];
            newColumn.ReadOnly = true;
            newColumn.Width = 150;

            grid.Columns.Add("Key", "Key");
            newColumn = grid.Columns[grid.Columns.Count - 1];
            newColumn.ReadOnly = true;
            newColumn.Width = 150;
        }

        #endregion

        #region Methods

        /// <summary>
        /// returns an interface by key attribute
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private XElement GetInterfaceNode(string key)
        {
            XElement documentNode = gridInherited.Tag as XElement;
            XElement projectsNode = documentNode.Element("Solution").Element("Projects");

            foreach (XElement projectNode in projectsNode.Elements())
            {
                var node = (from a in projectNode.Elements("DispatchInterfaces").Elements("Interface")
                            where a.Attribute("Key").Value.Equals(key)
                            select a).FirstOrDefault();
                if (node != null)
                    return node;

                node = (from a in projectNode.Elements("Interfaces").Elements("Interface")
                            where a.Attribute("Key").Value.Equals(key)
                            select a).FirstOrDefault();
                if (node != null)
                    return node;
            }

            throw (new ArgumentOutOfRangeException("key"));
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

        #endregion
    }
}
