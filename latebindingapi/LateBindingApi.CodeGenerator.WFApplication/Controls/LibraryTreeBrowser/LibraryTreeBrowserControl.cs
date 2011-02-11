using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LateBindingApi.CodeGenerator.WFApplication.Controls.LibraryTreeBrowser
{
    /// <summary>
    /// Shows document types in a grid
    /// </summary>
    public partial class LibraryTreeBrowserControl : UserControl
    {
        #region Fields
        
        private string _currentNodePath;

        #endregion

        #region Events
        
        /// <summary>
        /// Grid Trigger sfter select 
        /// </summary>
        public event TreeViewEventHandler AfterSelect;

        #endregion

        #region Construction

        public LibraryTreeBrowserControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods

        private void ShowElements(XElement project, TreeNode treeProject,string iteratorName, string elementName)
        {
            TreeNode treeIterator = treeProject.Nodes.Add(iteratorName, iteratorName);
            foreach (var itemEnum in project.Element(iteratorName).Elements(elementName))
            {
                string enumName = itemEnum.Attribute("Name").Value;
                if (true == FilterPassed(enumName))
                {
                    TreeNode treeElement = treeIterator.Nodes.Add(enumName, enumName);
                    treeElement.Tag = itemEnum;
                }
            }
        }

        public void Show(XElement node)
        {
            Clear();
            treeViewComponents.Tag = node;

            // show libraries
            TreeNode treeLibraries = treeViewComponents.Nodes.Add("Libraries");
            treeLibraries.Tag = node.Element("Libraries");

            // show projects
            TreeNode treeSolution = treeViewComponents.Nodes.Add("Solution");
            treeSolution.Expand();
            foreach (var item in node.Element("Solution").Element("Projects").Elements("Project"))
            {
                string projectName = item.Attribute("Name").Value;
                TreeNode treeProject = treeSolution.Nodes.Add(projectName, projectName);
                treeProject.Tag = item;

                ShowElements(item, treeProject, "Enums", "Enum");
                ShowElements(item, treeProject, "DispatchInterfaces", "Interface");
                ShowElements(item, treeProject, "Interfaces", "Interface");
                ShowElements(item, treeProject, "CoClasses", "CoClass");
            }
        }

        public void Clear()
        {
            treeViewComponents.Nodes.Clear();
        }
        
        #endregion

        #region Filter Gui Trigger

        private bool FilterPassed(string expression)
        {
            string filterText = textBoxFilter.Text.Trim();
            if (filterText == "") return true;

            if (expression.IndexOf(filterText, 0, StringComparison.InvariantCultureIgnoreCase) > -1)
                return true;
            else
                return false;
        }

        private void textBoxFilter_KeyDown(object sender, KeyEventArgs e)
        {
            XElement documentNode = treeViewComponents.Tag as XElement;
            if ((null != documentNode) && (e.KeyCode == Keys.Return))
            {
                SaveCurrentNodePath();
                Show(documentNode);
                RestoreExpandState();
            }
        }

        public void SaveCurrentNodePath()
        {
            if (null == treeViewComponents.SelectedNode)
                _currentNodePath = "";
            else
                _currentNodePath = treeViewComponents.SelectedNode.FullPath;
        }

        public void RestoreExpandState()
        {
            TreeNode node = null;
            string[] splitArray = _currentNodePath.Split(new string[] { treeViewComponents.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string nodeName in splitArray)
            {
                if (node == null)
                    node = SearchChildTree(treeViewComponents, nodeName);
                else
                    node = SearchChildTree(node, nodeName);

                if (node != null)
                    node.Expand();
            }
            treeViewComponents.SelectedNode = node;
        }

        private TreeNode SearchChildTree(TreeView treeView, string name)
        {
            foreach (TreeNode tn in treeViewComponents.Nodes)
            {
                if (tn.Text == name)
                    return tn;
            }
            return null;
        }

        private TreeNode SearchChildTree(TreeNode treeNode, string name)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                if (tn.Text == name)
                    return tn;
            }
            return null;
        } 

        #endregion

        #region Tree Gui Trigger

        private void treeViewComponents_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (null != AfterSelect)
                AfterSelect(sender, e);
        }

        #endregion
    }
}
