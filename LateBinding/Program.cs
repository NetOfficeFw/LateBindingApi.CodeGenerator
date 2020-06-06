using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LateBindingApi.CodeGenerator.ComponentAnalyzer;
using LateBindingApi.CodeGenerator.ComponentAnalyzer.Model;
using NLog;
using NLog.Targets;

namespace LateBinding
{
    public class Program
    {
        private static readonly Logger Log = LogManager.GetLogger(nameof(Program));

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            var setId = "365";
            if (args.Length == 1)
            {
                setId = args[0];
            }

            var office = GetLibrarySet(setId);

            if (!CheckLibrariesExist(office.Libraries))
            {
                return 1;
            }

            var sourcePath = Path.Combine(Environment.CurrentDirectory, "NetOffice.xml");
            var targetFilename = $"NetOffice-Office{setId}.xml";
            var targetPath = Path.Combine(Environment.CurrentDirectory, targetFilename);

            ////DumpOfficeVersions(office);

            var comAnalyzer = new Analyzer();
            comAnalyzer.DefaultLibraryVersion = office.VersionName;
            if (File.Exists(sourcePath))
            {
                comAnalyzer.LoadProject(sourcePath);
            }

            comAnalyzer.Finish += (timeElapsed) => { Log.Info($"Done loading library.\nTime: {timeElapsed}"); };

            try
            {
                comAnalyzer.LoadTypeLibraries(office.Libraries, true, false);
                comAnalyzer.SaveProject(targetPath);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Failed to load type library information.");
            }

            Log.Info("Done.");

            if (Debugger.IsAttached)
            {
                Console.ReadKey();
            }

            return 0;
        }

        private static bool CheckLibrariesExist(IEnumerable<string> libraries)
        {
            foreach (var library in libraries)
            {
                if (!File.Exists(library))
                {
                    Log.Error($"Library {library} does not exists.");
                    return false;
                }

                var filename = Path.GetFileName(library);
                Log.Info($"Library {filename} exists at {library}");
            }

            return true;
        }

        private static void DumpOfficeVersions(OfficeProduct office)
        {
            var comAnalyzer = new Analyzer();
            comAnalyzer.DefaultLibraryVersion = office.VersionName;

            var libraries = comAnalyzer.LoadLibraryVersions(office);

            Console.WriteLine($"Product: {office.Name}");
            Console.WriteLine($"Release: {office.ReleaseName}");
            Console.WriteLine($"Version: {office.VersionName}");

            foreach (var library in libraries.OrderBy(k => k.LibraryName))
            {
                Console.WriteLine($"  - Library: {library.LibraryName}");
                Console.WriteLine($"    ProductVersion: {library.Version}");
                Console.WriteLine($"    LibraryVersion: {library.Major}.{library.Minor}");
            }
        }


        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Log.Error(ex, $"Unhandled exception in latebinding.exe (IsTerminating={e.IsTerminating}).");
        }

        private static OfficeProduct GetLibrarySet(string id)
        {
            switch(id)
            {
                case "2013":
                    return GetMsOffice2013LibrarySet();
                case "2016":
                    return GetMsOffice2016LibrarySet();
                case "365":
                    return GetMsOffice365LibrarySet();
                default:
                    throw new ArgumentOutOfRangeException($"Unknown library set id '{id}'");
            }
        }

        private static OfficeProduct GetMsOffice2013LibrarySet()
        {
            var access_v2013 = @"C:\Program Files\Microsoft Office\Office15\MSACC.OLB";
            var excel_v2013 = @"C:\Program Files\Microsoft Office\Office15\EXCEL.EXE";
            var msproject_v2013 = @"C:\Program Files\Microsoft Office\Office15\MSPRJ.OLB";
            var office_v2013 = @"C:\Program Files\Common Files\Microsoft Shared\OFFICE15\MSO.DLL";
            var outlook_v2013 = @"C:\Program Files\Microsoft Office\Office15\MSOUTL.OLB";
            var powerpoint_v2013 = @"C:\Program Files\Microsoft Office\Office15\MSPPT.OLB";
            var publisher_v2013 = @"C:\Program Files\Microsoft Office\Office15\MSPUB.TLB";
            var visio_v2013 = @"C:\Program Files\Microsoft Office\Office15\VISLIB.DLL";
            var word_v2013 = @"C:\Program Files\Microsoft Office\Office15\MSWORD.OLB";

            var libs = new String[] { office_v2013, excel_v2013, word_v2013, outlook_v2013, powerpoint_v2013, access_v2013, msproject_v2013, visio_v2013, publisher_v2013 };


            var office2013 = new OfficeProduct
            {
                Name = "Microsoft Office 2013",
                VersionName = "15",
                ReleaseName = "2013",
                Libraries = libs
            };
            return office2013;
        }

        private static OfficeProduct GetMsOffice2016LibrarySet()
        {
            var office_v2016 = @"C:\Program Files\Microsoft Office\root\VFS\ProgramFilesCommonX86\Microsoft Shared\OFFICE16\MSO.DLL";
            var vbIDE_v2016 = @"C:\Program Files\Microsoft Office\Root\VFS\ProgramFilesCommonX86\Microsoft Shared\VBA\VBA6\VBE6EXT.OLB";
            var excel_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\EXCEL.EXE";
            var word_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSWORD.OLB";
            var outlook_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSOUTL.OLB";
            var powerPoint_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSPPT.OLB";
            var access_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSACC.OLB";
            var msProject_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSPRJ.OLB";
            var publisher_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSPUB.TLB";
            var visio_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\VISLIB.DLL";

            var libs = new String[] { office_v2016, excel_v2016, word_v2016, outlook_v2016, powerPoint_v2016, access_v2016, msProject_v2016, vbIDE_v2016, publisher_v2016, visio_v2016 };

            var office2016 = new OfficeProduct
            {
                Name = "Microsoft Office 2016",
                VersionName = "16",
                ReleaseName = "2016",
                Libraries = libs
            };
            return office2016;
        }

        private static OfficeProduct GetMsOffice365LibrarySet()
        {
            var office_v2016 = @"C:\Program Files\Microsoft Office\root\VFS\ProgramFilesCommonX86\Microsoft Shared\OFFICE16\MSO.DLL";
            var vbIDE_v2016 = @"C:\Program Files\Microsoft Office\Root\VFS\ProgramFilesCommonX86\Microsoft Shared\VBA\VBA6\VBE6EXT.OLB";
            var excel_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\EXCEL.EXE";
            var word_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSWORD.OLB";
            var outlook_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSOUTL.OLB";
            var powerPoint_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSPPT.OLB";
            var access_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSACC.OLB";
            var msProject_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSPRJ.OLB";
            var publisher_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\MSPUB.TLB";
            var visio_v2016 = @"C:\Program Files\Microsoft Office\Root\Office16\VISLIB.DLL";

            var libs = new String[] { office_v2016, excel_v2016, word_v2016, outlook_v2016, powerPoint_v2016, access_v2016, msProject_v2016, vbIDE_v2016, publisher_v2016, visio_v2016 };

            var office365 = new OfficeProduct
            {
                Name = "Microsoft Office 365",
                VersionName = "16.1",
                ReleaseName = "2005",
                Libraries = libs
            };
            return office365;
        }
    }
}
