using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

using LateBindingApi.CodeGenerator.ComponentAnalyzer;

namespace LateBindingApi.CodeGenerator.CSharp
{
    public class CSharpGenerator : ICodeGenerator
    {
        #region Fields

        Settings _settings;
        
        #endregion

        #region ICodeGenerator Members

        public string Name
        {
            get 
            {
                return "C#";
            }
        }

        public string Description
        {
            get
            {
                return "Creates a VS Solution with C# Projects (.csproj)";
            }
        }

        public Version Version
        {
            get
            {
                return new Version("1.0");
            }
        }

        public DialogResult ShowConfigDialog(Control parentDialog)
        {
            FormConfigDialog formConfig = new FormConfigDialog();
            DialogResult dr = formConfig.ShowDialog(parentDialog);
            if (dr == DialogResult.OK)
            {
                _settings = formConfig.Selected;
                return dr;
            }
            else
                return dr;
        }

        public void Generate(XDocument document)
        {
            MessageBox.Show("Generate with setting" + _settings.Folder); 
        }

        #endregion
    }
}
