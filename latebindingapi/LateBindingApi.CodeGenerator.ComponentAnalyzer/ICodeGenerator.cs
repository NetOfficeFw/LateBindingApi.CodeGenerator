using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public interface ICodeGenerator
    {

        string Name { get; }
        string Description { get; }
        Version Version { get; }

        /// <summary>
        /// display config dialog. returns DialogResult.Ok or DialogResult.Cancel
        /// </summary>
        /// <param name="parentDialog"></param>
        /// <returns></returns>
        DialogResult ShowConfigDialog(Control parentDialog);

        /// <summary>
        /// generates given document to solution
        /// </summary>
        /// <param name="document"></param>
        void Generate(XDocument document);
    }
}
