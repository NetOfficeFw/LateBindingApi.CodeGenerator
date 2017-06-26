using System;
using NLog;

namespace LateBindingApi.CodeGenerator.CodeGen
{
    class Program
    {
        private static readonly Logger Log = LogManager.GetLogger(nameof(Program));

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

            Options options;
            if (!Options.TryParse(args, out options))
            {
                Log.Error("Failed to parse command line arguments.");
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            Log.Info($@"Generating code to '{options.OutputFolder}'");
            Log.Info("Done.");
            Console.ReadKey();
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            Log.Error(ex, $"Unhandled exception in codegen.exe (IsTerminating={e.IsTerminating}).");
        }
    }
}
