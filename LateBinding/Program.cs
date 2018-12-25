using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LateBindingApi.CodeGenerator.ComponentAnalyzer;
using NLog;

namespace LateBinding
{
    public class Program
    {
        private static readonly Logger Log = LogManager.GetLogger(nameof(Program));

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            var officeLib_v2016 = @"C:\Program Files (x86)\Microsoft Office\Root\VFS\ProgramFilesCommonX86\Microsoft Shared\OFFICE16\MSO.DLL";
            var vbIDELib_v2016 = @"C:\Program Files (x86)\Microsoft Office\Root\VFS\ProgramFilesCommonX86\Microsoft Shared\VBA\VBA6\VBE6EXT.OLB";
            var excelLib_v2016 = @"C:\Program Files (x86)\Microsoft Office\Root\Office16\EXCEL.EXE";
            var wordLib_v2016 = @"C:\Program Files (x86)\Microsoft Office\Root\Office16\MSWORD.OLB";
            var outlookLib_v2016 = @"C:\Program Files (x86)\Microsoft Office\Root\Office16\MSOUTL.OLB";
            var powerPointLib_v2016 = @"C:\Program Files (x86)\Microsoft Office\Root\Office16\MSPPT.OLB";
            var accessLib_v2016 = @"C:\Program Files (x86)\Microsoft Office\Root\Office16\MSACC.OLB";
            var msProjectLib_v2016 = @"C:\Program Files (x86)\Microsoft Office\Root\Office16\MSPRJ.OLB";

            //var libs = new String[] { officeLib_v2016, excelLib_v2016, wordLib_v2016, outlookLib_v2016, powerPointLib_v2016, accessLib_v2016, msProjectLib_v2016, vbIDELib_v2016 };
            var libs = new String[] { officeLib_v2016 };

            var comAnalyzer = new Analyzer();
            comAnalyzer.DefaultLibraryVersion = "16";
            comAnalyzer.LoadProject(@"c:\dev\netoffice\NetOffice.xml");
            comAnalyzer.Update += (sender, message) => { Log.Info(message); };
            comAnalyzer.Finish += (timeElapsed) => { Log.Info($"Done loading library.\nTime: {timeElapsed}"); };

            try
            {
                comAnalyzer.LoadTypeLibraries(libs, true, false);
                comAnalyzer.SaveProject(@"c:\dev\netoffice\NetOffice-Office2016.xml");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to load type library information.");
            }

            Log.Info("Done.");

            //if (Debugger.IsAttached)
            {
                Console.ReadKey();
            }
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Log.Error(ex, $"Unhandled exception in codegen.exe (IsTerminating={e.IsTerminating}).");
        }
    }
}
