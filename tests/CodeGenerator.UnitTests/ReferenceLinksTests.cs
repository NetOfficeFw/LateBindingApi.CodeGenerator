using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        public async Task T()
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
    }
}
