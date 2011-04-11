using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public partial class HelpBox : UserControl
    {
        Control _parent;

        public HelpBox(Control parent)
        {
            InitializeComponent();
            _parent = parent;
        }

        public void Show(string messageId)
        {
            foreach (Control item in _parent.Controls)
                item.Visible = false;

            string helpText = RessourceApi.ReadString("Help.HelpText.txt");
            richTextBoxMessage.Text = GetHelpMessage(helpText, messageId);

            _parent.Controls.Add(this);
            this.Dock = DockStyle.Fill;
            this.Visible = true;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            _parent.Controls.Remove(this);
            foreach (Control item in _parent.Controls)
                item.Visible = true;
        }


        private string GetHelpMessage(String helpText, string messageId)
        {
            int i = helpText.IndexOf(messageId);
            helpText = helpText.Substring(i +messageId.Length+"\r\n".Length);
            i = helpText.IndexOf("#\r\n");
            helpText = helpText.Substring(0,i);
            return helpText;
        }
    }
}
