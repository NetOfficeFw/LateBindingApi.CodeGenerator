using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LateBindingApi.CodeGenerator.ComponentAnalyzer
{
    public delegate void ICodeGeneratorProgressHandler(string message);

    public interface ICodeGenerator
    {
        string Name { get; }
        string Description { get; }
        Version Version { get; }
        bool IsAlive { get; }

        /// <summary>
        /// Cancel async operaton
        /// </summary>
        void Abort();

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
        Task<TimeSpan> Generate(XDocument document);

        /// <summary>
        /// progress information from worker thread
        /// </summary>
        event ICodeGeneratorProgressHandler Progress;
    }
}
