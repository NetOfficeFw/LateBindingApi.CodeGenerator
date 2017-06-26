using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;

namespace LateBindingApi.CodeGenerator.CodeGen
{
    public class Options
    {
        [Option('o', "output", DefaultValue = "out", HelpText = "Output folder for generated code")]
        public string OutputFolder { get; set; }

        public static bool TryParse(string[] args, out Options options)
        {
            options = new Options();
            if (args != null)
            {
                return Parser.Default.ParseArguments(args, options);
            }

            return true;
        }
    }
}
