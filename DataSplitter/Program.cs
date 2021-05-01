using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            using var repo = new Repository(SourceRepo);
            Commands.Checkout(repo, "main");

            var filter = new CommitFilter()
            {
                SortBy = CommitSortStrategies.Topological
            };
            var allCommits = repo.Commits.QueryBy(filter);
            var commits = allCommits.Reverse().Skip(1).SkipLast(5).ToList();

            foreach (var commit in commits)
            {
                await SplitProjects(repo, commit);
            }

            Console.WriteLine("Done.");

            return 0;
        }

        private static async Task SplitProjects(Repository repo, Commit commit)
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

            Console.WriteLine($"Commit '{commit.Message}'");

            foreach (var project in projects)
            {
                Console.WriteLine($"Project {project}");
                await SplitProject(project);
            }

            using var repo2 = new Repository(TargetRepo);
            Commands.Stage(repo2, "*");
            if (repo2.HasChanges())
            {
                var signature = new Signature("Jozef Izso", "jozef.izso@gmail.com", DateTimeOffset.Now);
                repo2.Commit(commit.Message, signature, signature);
            }
        }

        private static async Task SplitProject(string projectName)
        {
            var sourceProjectFilename = $"Project.{projectName}.xml";
            var source = Path.Combine(SourceRepo, "src", sourceProjectFilename);
            var targetFolder = Path.Combine(TargetRepo, "src", projectName);
            var targetProjectPath = Path.Combine(targetFolder, $"Project.xml");

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

            var prefix = "  -> ";

            foreach (var dataType in dataTypes)
            {
                var typeName = dataType.Name.LocalName;

                var typeFilename = $"{typeName}.xml";
                var typeTarget = Path.Combine(targetFolder, typeFilename);

                Console.Write($"{prefix} {typeFilename}");
                prefix = ", ";

                using var fs = new FileStream(typeTarget, FileMode.Create);
                await dataType.SaveAsync(fs, SaveOptions.None, CancellationToken.None);
                await fs.FlushAsync();
            }

            projectElement.Add(new XAttribute(XNamespace.Xmlns + "xi", "http://www.w3.org/2001/XInclude"));

            foreach (var dataType in dataTypes)
            {
                var typeName = dataType.Name.LocalName;
                var dataTypeLink = new XElement(xi + "include", new XAttribute("href", $"{typeName}.xml"));
                dataType.ReplaceWith(dataTypeLink);
            }
            
            using var fsp = new FileStream(targetProjectPath, FileMode.Create);
            await projectElement.SaveAsync(fsp, SaveOptions.None, CancellationToken.None);
            await fsp.FlushAsync();

            Console.WriteLine($". Done.");
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
