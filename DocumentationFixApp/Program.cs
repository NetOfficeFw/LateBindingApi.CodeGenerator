using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;

namespace DocumentationFixApp
{
    class Program
    {
        private static HttpClient httpClient;

        static void Main(string[] args)
        {
            var referenceIndexFile = args[0];
            if (!Path.IsPathRooted(referenceIndexFile))
            {
                referenceIndexFile = Path.Combine(Environment.CurrentDirectory, referenceIndexFile);
            }

            var document = XDocument.Load(referenceIndexFile);
            var linkElements = document.Descendants("Link");

            httpClient = new HttpClient();


            ServicePointManager.DefaultConnectionLimit = 20;
            var blockOptions = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 10 };
            var actionBlock = new ActionBlock<XElement>(GetNewUrlForLinkElement, blockOptions);

            foreach (var linkElement in linkElements)
            {
                actionBlock.Post(linkElement);
            }

            actionBlock.Complete();
            actionBlock.Completion.Wait();

            var newFilepath = Path.Combine(Path.GetDirectoryName(referenceIndexFile), Path.GetFileNameWithoutExtension(referenceIndexFile) + "_new.xml");
            document.Save(newFilepath);

            Console.ReadKey();
        }

        private static async Task GetNewUrlForLinkElement(XElement linkElement)
        {
            var elementName = ((XElement)linkElement.PreviousNode).Value;
            var url = linkElement.Value;
            var newUrl = await GetNewUrl(elementName, url, httpClient);
            if (newUrl != null)
            {
                Console.WriteLine($"{elementName} => {newUrl}");

                linkElement.Value = newUrl;
            }
        }

        static async Task<string> GetNewUrl(string elementName, string url, HttpClient httpClient)
        {
            try
            {
                Console.WriteLine($"Loading {elementName} data...");

                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var newUrl = response.RequestMessage.RequestUri.ToString();
                    return newUrl;
                }
                else
                {
                    Console.WriteLine($"Error for {elementName}: {response.StatusCode:D} {response.StatusCode})");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to load new URL for resource {url}. Exception: {e.Message}");
            }

            return null;
        }
    }
}
