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


namespace LateBindingApi.CodeGenerator.WFApplication.Controls.SourceEdit
{
    public partial class SourceEditControl : UserControl
    {
        #region Fields

        bool _showFlag;         // stores grid is currently filled,no events fire

        #endregion

        #region Construction

        public SourceEditControl()
        {
            InitializeComponent();
        }
        
        #endregion

        #region ControlMethods

        public void Show(XElement node)
        {
            _showFlag = true;

            Clear();

            textBoxItem.Text = node.ToString();
            textBoxItem.Tag = node;

            _showFlag = false;
        }

        public void Clear()
        {
            textBoxSearch.Text = ""; 
            textBoxItem.Text = "";
            buttonReset.Enabled = false;
            buttonApply.Enabled = false; 
        }



        #endregion

        #region Gui Trigger
        
        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            string searchText = textBoxSearch.Text.Trim();
            if( (e.KeyData == Keys.Return) && (searchText != "") )
            {
                int startPosition = textBoxItem.SelectionStart+1;
                int foundPosition = textBoxItem.Text.IndexOf(searchText ,startPosition, StringComparison.InvariantCultureIgnoreCase);                   
                if(foundPosition > -1)
                {
                    textBoxItem.SelectionStart = foundPosition;
                    textBoxItem.SelectionLength = searchText.Length;
                    textBoxItem.ScrollToCaret();
                }
                else
                {
                    textBoxItem.SelectionStart=0;
                }
            }
        }

        private void textBoxItem_TextChanged(object sender, EventArgs e)
        {
            if (false == _showFlag)
            { 
                buttonReset.Enabled = true;
                buttonApply.Enabled = true;
            }
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            _showFlag = true;
            
            XElement node = textBoxItem.Tag as XElement;
            textBoxItem.Text = node.ToString();
            textBoxItem.SelectionStart = 0;

            buttonReset.Enabled = false;
            buttonApply.Enabled = false;

            _showFlag = false;
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            _showFlag = true;          

            XElement node = textBoxItem.Tag as XElement;
            XElement parent = node.Parent;
            string key = node.Attribute("Key").Value;
            string xmlContent = textBoxItem.Text;
            node.ReplaceWith(XElement.Parse(xmlContent));
            node = (from a in parent.Elements()
                        where a.Attribute("Key").Value.Equals(key)
                        select a).FirstOrDefault();

            textBoxItem.Tag = node;
           
            buttonReset.Enabled = false;
            buttonApply.Enabled = false;

            _showFlag = false;
        }

        #endregion
    }   
}
