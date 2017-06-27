using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace LateBindingApi.CodeGenerator.CodeGen
{
    public class Options
    {
        [Option('p', "project", Required = true, HelpText = "Path to project file.")]
        public string ProjectFile { get; set; }

        [Option('r', "ref", Required = true, HelpText = "Path to reference analyzer index file.")]
        public string ReferenceFile { get; set; }

        [Option('k', "keyfiles", Required = true, HelpText = "Path to directory with key files.")]
        public string KeyFilesFolder { get; set; }

        [Option('o', "output", DefaultValue = "out", HelpText = "Output folder path where code will be generated.")]
        public string OutputFolder { get; set; }

        public static bool TryParse(string[] args, out Options options)
        {
            options = new Options();
            if (args != null)
            {
                if (Parser.Default.ParseArguments(args, options))
                {
                    if (!Path.IsPathRooted(options.ProjectFile))
                    {
                        options.ProjectFile = Path.GetFullPath(options.ProjectFile);
                    }

                    if (!Path.IsPathRooted(options.ReferenceFile))
                    {
                        options.ReferenceFile = Path.GetFullPath(options.ReferenceFile);
                    }
                    if (File.GetAttributes(options.ReferenceFile) == FileAttributes.Directory)
                    {
                        options.ReferenceFile = Path.Combine(options.ReferenceFile, "ReferenceIndex2.xml");
                    }

                    if (!Path.IsPathRooted(options.KeyFilesFolder))
                    {
                        options.KeyFilesFolder = Path.GetFullPath(options.KeyFilesFolder);
                    }

                    if (!Path.IsPathRooted(options.OutputFolder))
                    {
                        options.OutputFolder = Path.GetFullPath(options.OutputFolder);
                    }
                }
            }

            return true;
        }
    }
}
