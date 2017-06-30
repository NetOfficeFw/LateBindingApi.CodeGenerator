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
            analyzer.LoadProject(options.ProjectFile);
            sw.Stop();

            Log.Info($@"Project file loaded in {sw.Elapsed} ({sw.ElapsedMilliseconds}ms).");

            if (Directory.Exists(options.OutputFolder))
            {
                Directory.Delete(options.OutputFolder, recursive: true);
            }

            var csSettings = new Settings();
            csSettings.Folder = options.OutputFolder;
            csSettings.LinkFilePath = options.ReferenceFile;
            csSettings.SignPath = options.KeyFilesFolder;
            csSettings.Framework = "4.0";

            csSettings.AddDocumentationLinks = true;
            csSettings.ConvertOptionalsToObject = false;
            csSettings.ConvertParamNamesToCamelCase = true;
            csSettings.CreateXmlDocumentation = true;
            csSettings.RemoveRefAttribute = true;
            csSettings.UseSigning = true;

            csSettings.VBOptimization = false;
            csSettings.AddTestApp = false;
            csSettings.OpenFolder = false;

            Log.Info($@"Generating code to '{options.OutputFolder}'");

            CSharpGenerator.Settings = csSettings;
            var generator = new CSharpGenerator();
            generator.Progress = new Progress<string>(GeneratorProgress);
            var elapsedTime = await generator.Generate(analyzer.Document, CancellationToken.None);
            Log.Info($@"Code generated in {elapsedTime} ({elapsedTime.TotalMilliseconds}ms).");

            Log.Info("Done.");

            if (Debugger.IsAttached)
            {
                Console.ReadKey();
            }
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
