using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using NUnit.Framework;

namespace CodeGenerator.UnitTests
{
    [TestFixture]
    public class ReferenceLinksTests
    {
        private ConcurrentDictionary<string, string> failed = new ConcurrentDictionary<string, string>();

        [Test]
        public async Task UpdateMsdnLinksToMicrosoftDocs()
        {
            var file = @"c:\dev\github\NetOfficeFw\NetOffice-Data\bld\ReferenceIndex2.xml";
            var xml = XDocument.Load(file);

            var http = new HttpClient();
            http.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36");

            var options = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 16
            };

            var worker = new ActionBlock<XElement>(link => UpdateLink(http, link), options);

            foreach(var link in xml.Descendants("Link"))
            {
                worker.Post(link);
            }

            worker.Complete();

            await worker.Completion;

            xml.Save(@"c:\dev\github\NetOfficeFw\NetOffice-Data\bld\ReferenceIndex3.xml");
        }

        public async Task UpdateLink(HttpClient client, XElement link)
        {
            var oldUrl = link.Value;

            try
            {
                var x = await client.GetAsync(oldUrl);
                if (x.IsSuccessStatusCode)
                {
                    var newLocation = x.RequestMessage.RequestUri;
                    var oldLink = new XElement("MsdnLink");
                    oldLink.Value = link.Value;

                    link.Value = newLocation.AbsoluteUri;
                    link.Parent?.Add(oldLink);
                }
                else
                {
                    this.failed[oldUrl] = $"{x.StatusCode}";
                }
            }
            catch (Exception e)
            {
                this.failed[oldUrl] = e.ToString();
            }
        }

        private static readonly CultureInfo US = new CultureInfo("en-US");
        private static readonly UTF8Encoding UTB8NoBom = new UTF8Encoding(false);

        [Test]
        public void ConvertMsdnLinksToMicrosoftDocs()
        {
            var linkRegex = new Regex(@"http(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?", RegexOptions.Compiled);
            var links = new Dictionary<string, string>();

            var path = @"c:\dev\github\NetOfficeFw\NetOffice-Data\bld";
            var netofficePath = @"c:\dev\github\NetOfficeFw\NetOffice\Source";
            var files = Directory.EnumerateFiles(path, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var xml = XDocument.Load(file);

                foreach (var msdnLink in xml.Descendants("MsdnLink"))
                {
                    var oldLink = msdnLink.Value;
                    var link = msdnLink.PreviousNode as XElement;
                    if (link?.Name.LocalName != "Link")
                    {
                        link = msdnLink.Parent?.Element("Link");
                    }

                    if (link != null)
                    {
                        var newLink = link.Value;
                        links[oldLink] = newLink.ToLower(US);
                    }
                }
            }

            var sourceFiles = Directory.EnumerateFiles(netofficePath, "*.cs", SearchOption.AllDirectories);
            foreach (var file in sourceFiles)
            {
                var bom = new byte[3];
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(bom, 0, 3);
                }

                var source = File.ReadAllText(file);

                bool hasBom = bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF;

                var newSource = linkRegex.Replace(source, match =>
                {
                    var oldLink = match.Value;
                    if (links.TryGetValue(oldLink, out var newLink))
                    {
                        return newLink;
                    }

                    return oldLink;
                });

                File.WriteAllText(file, newSource, hasBom ? Encoding.UTF8 : UTB8NoBom);
            }
        }

        [Test]
        public void FixUtf8Bom()
        {
            var netofficePath = @"c:\dev\github\NetOfficeFw\NetOffice";

            var ext = new[] {".cs", ".vb", ".vbproj", ".csproj", ".sln", ".resx", ".config", ".xml", ".txt", ".settings"};
            var sourceFiles = Directory.EnumerateFiles(netofficePath, "*.*", SearchOption.AllDirectories)
                .Where(x => !x.Contains(@".git\"))
                .Where(x => ext.Any(x.EndsWith));

            foreach (var file in sourceFiles)
            {
                var bom = new byte[3];
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    fs.Read(bom, 0, 3);
                }

                bool hasBom = bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF;

                if (!hasBom)
                {
                    var content = File.ReadAllText(file);
                    File.WriteAllText(file, content, Encoding.UTF8);
                }
            }
        }

        [Test]
        public void FileTypes()
        {
            var netofficePath = @"c:\dev\github\NetOfficeFw\NetOffice";

            var sourceFiles = Directory.EnumerateFiles(netofficePath, "*.*", SearchOption.AllDirectories)
                .Where(x => !x.Contains(@".git\"));

            var h = new Histogram();

            foreach (var file in sourceFiles)
            {
                var ext = Path.GetExtension(file).TrimStart('.').ToLower();
                h.Increment(ext);
            }

            var occurence = h.ToString();
        }

        private class Histogram
        {
            private readonly Dictionary<string, int> c = new Dictionary<string, int>(50);

            public void Increment(string extension)
            {
                if (c.TryGetValue(extension, out var value))
                {
                    c[extension] = value + 1;
                }
                else
                {
                    c[extension] = 1;
                }
            }

            public override string ToString()
            {
                var sb = new StringBuilder(c.Count * 20);
                foreach (var kv in c)
                {
                    sb.AppendLine($"{kv.Key}: {kv.Value}");
                }

                return sb.ToString();
            }
        }
    }
}
