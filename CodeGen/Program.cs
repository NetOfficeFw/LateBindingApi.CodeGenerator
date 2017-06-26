using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using LateBindingApi.CodeGenerator.ComponentAnalyzer;
using NLog;

using LateBindingApi.CodeGenerator.CSharp;

namespace LateBindingApi.CodeGenerator.CodeGen
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetLogger(nameof(Program));

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            Options options;
            if (!Options.TryParse(args, out options))
            {
                Log.Error("Failed to parse command line arguments.");
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            Log.Info("Loading project file...");

            var analyzer = new Analyzer();

            var sw = Stopwatch.StartNew();
            analyzer.LoadProject(@"d:\dev\github\NetOfficeFw\NetOffice-ReferenceApi\NetOffice 1.7.4.xml");
            sw.Stop();

            Log.Info($@"Project file loaded in {sw.Elapsed} ({sw.ElapsedMilliseconds}ms).");


            var outFolder = Path.GetFullPath(options.OutputFolder);
            if (!Directory.Exists(outFolder))
            {
                Directory.CreateDirectory(outFolder);
            }

            var csSettings = new Settings();
            csSettings.Folder = outFolder;
            csSettings.LinkFilePath = @"d:\dev\github\NetOfficeFw\NetOffice-ReferenceApi\ReferenceIndex2.xml";
            csSettings.SignPath = @"c:\dev\github\NetOfficeFw\NetOffice-old\KeyFiles\4.5";
            csSettings.Framework = "4.5";

            csSettings.AddDocumentationLinks = true;
            csSettings.AddTestApp = false;
            csSettings.ConvertOptionalsToObject = false;
            csSettings.ConvertParamNamesToCamelCase = true;
            csSettings.CreateXmlDocumentation = true;
            csSettings.OpenFolder = false;
            csSettings.RemoveRefAttribute = true;
            csSettings.UseSigning = true;
            csSettings.VBOptimization = false;

            Log.Info($@"Generating code to '{options.OutputFolder}'");

            CSharpGenerator.Settings = csSettings;
            var generator = new CSharpGenerator();
            generator.Progress = new Progress<string>(GeneratorProgress);
            var elapsedTime = await generator.Generate(analyzer.Document, CancellationToken.None);
            Log.Info($@"Code generated in {elapsedTime} ({elapsedTime.TotalMilliseconds}ms).");

            Log.Info("Done.");
            Console.ReadKey();
        }

        private static void GeneratorProgress(string message)
        {
            Log.Warn(message);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Log.Error(ex, $"Unhandled exception in codegen.exe (IsTerminating={e.IsTerminating}).");
        }
    }
}
