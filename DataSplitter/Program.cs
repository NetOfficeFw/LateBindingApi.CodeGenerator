using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using LibGit2Sharp;

namespace DataSplitter
{
    class Program
    {
        const string SourceRepo = @"d:\dev\github\NetOfficeFw\Data";
        const string TargetRepo = @"d:\dev\github\NetOfficeFw\Data2";

        static async Task<int> Main(string[] args)
        {
            var commits = new[] {
                "aeda0741974e9fd0b67b4fc2083633eedd25ba3e",
                "d720b3147f8be1f388b96e8c00fd1378a75a151e",
                "1afb8214c34173525228f68df6811e27ea6a8c04",
                "122c59771be0fceeed58370edf95ef86d16ff4d6"
            };

            using var repo = new Repository(SourceRepo);

            await SplitProjects(repo, commits[0]);

            Console.WriteLine("Done.");

            return 0;
        }

        private static async Task SplitProjects(Repository repo, string commit)
        {
            Commands.Checkout(repo, commit);

            EnsureDirectory(Path.Combine(TargetRepo, "src"));

            CopyFile(".gitignore");
            CopyFile("README.md");
            CopyFile(@"src\Libraries.xml");

            var projects = new[]
            {
                "MSHTML",
                "Excel",
                "Word",
                "MSProject",
                "Access",
                "Visio",
                "Outlook",
                "Office",
                "PowerPoint",
                "Publisher",
                "OWC10",
                "MSForms",
                "ADODB",
                "MSComctlLib",
                "DAO",
                "VBIDE",
                "stdole",
                "MSDATASRC"
            };

            foreach (var project in projects)
            {
                Console.WriteLine($"Project {project}");
                await SplitProject(project);
            }
        }

        private static async Task SplitProject(string projectName)
        {
            var projectFilename = $"Project.{projectName}.xml";
            var source = Path.Combine(SourceRepo, "src", projectFilename);
            var targetFolder = Path.Combine(TargetRepo, "src", projectName);

            if (!File.Exists(source))
            {
                Console.WriteLine($"  Project is empty.");
                return;
            }

            EnsureDirectory(targetFolder);

            var xi = (XNamespace)"http://www.w3.org/2001/XInclude";
            var doc = XDocument.Load(source);
            var projectElement = doc.Root;
            var dataTypes = projectElement.Elements().ToList();

            foreach (var dataType in dataTypes)
            {
                var typeName = dataType.Name.LocalName;

                var typeFilename = $"{typeName}.xml";
                var typeTarget = Path.Combine(targetFolder, typeFilename);

                Console.WriteLine($"  -> {typeFilename}");

                using var fs = new FileStream(typeTarget, FileMode.Create);
                await dataType.SaveAsync(fs, SaveOptions.None, CancellationToken.None);
                await fs.FlushAsync();
            }

            projectElement.Add(new XAttribute(XNamespace.Xmlns + "xi", "http://www.w3.org/2001/XInclude"));

            foreach (var dataType in dataTypes)
            {
                var typeName = dataType.Name.LocalName;
                var dataTypeLink = new XElement(xi + "include", new XAttribute("href", $"{projectName}/{typeName}.xml"));
                dataType.ReplaceWith(dataTypeLink);
            }
            
            var targetProjectFilename = Path.Combine(TargetRepo, "src", $"Project.{projectName}.xml");
            using var fsp = new FileStream(targetProjectFilename, FileMode.Create);
            await projectElement.SaveAsync(fsp, SaveOptions.None, CancellationToken.None);
            await fsp.FlushAsync();

            Console.WriteLine($"  Project done.");
        }

        private static void CopyFile(string filename)
        {
            var source = Path.Combine(SourceRepo, filename);
            var target = Path.Combine(TargetRepo, filename);

            File.Copy(source, target, overwrite: true);
        }

        private static void EnsureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
