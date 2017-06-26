using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        /// <param name="token"></param>
        Task<TimeSpan> Generate(XDocument document, CancellationToken token);

        /// <summary>
        /// progress information from worker thread
        /// </summary>
        IProgress<string> Progress { get; set; }
    }
}
