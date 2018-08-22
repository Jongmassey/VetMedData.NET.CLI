using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace VetMedData.NET.Util
{
    // ReSharper disable once InconsistentNaming
    public class EPARTools
    {
        private static readonly Uri EmaBaseUri = new Uri("http://www.ema.europa.eu/ema/");

        private const string EmaSearchUrl =
            @"index.jsp?curl=pages%2Fmedicines%2Flanding%2Fvet_epar_search.jsp&mid=WC0b01ac058001fa1c&searchTab=searchByKey&alreadyLoaded=true&isNewQuery=true&status=Authorised&status=Withdrawn&status=Suspended&status=Refused&keyword={prodname}&keywordSearch=Submit&searchType=name&taxonomyPath=&treeNumber=";

        private static bool IsEPAR(string url)
        {
            return url.Contains(@"ema.europa.eu/ema");
        }

        /// <summary>
        /// Progressively reduce
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        public static async Task<string[]> GetSearchResults(string productName)
        {
            var res = new string[0];

            do
            {
                res = await GetSearchResultsInternal(productName);
                productName = string.Join(' ', productName.Split(' ').Take(productName.Split(' ').Length - 1));
                if (string.IsNullOrWhiteSpace(productName))
                {
                    break;
                }
            } while (res.Length == 0);

            return res;
        }

        private static async Task<string[]> GetSearchResultsInternal(string productName)
        {
            using (var cli = new HttpClient())
            {
                cli.BaseAddress = EmaBaseUri;
                var innerlinks = new List<string>();
                var res = await cli.GetAsync(EmaSearchUrl.Replace("{prodname}", productName));

                var doc = new HtmlDocument();
                doc.Load(await res.Content.ReadAsStreamAsync());

                var outerlinks = doc.DocumentNode.SelectNodes("//a[@href]");
                foreach (var outerlink in outerlinks.Where(n =>
                    n.InnerText.Equals(productName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var innerres = await cli.GetAsync(outerlink.Attributes["href"].Value);

                    var innerdoc = new HtmlDocument();
                    innerdoc.Load(await innerres.Content.ReadAsStreamAsync());
                    var doclinks = innerdoc.DocumentNode.SelectNodes("//a[@href]")
                        .Where(n =>
                            n.InnerText.Contains("EPAR - Product Information") &&
                            n.Attributes["href"].Value.Contains("en_GB") &&
                            n.Attributes["href"].Value.EndsWith(".pdf"))
                        .Select(n => $"http://{EmaBaseUri.Host}{n.Attributes["href"].Value}");

                    innerlinks.AddRange(doclinks);
                }

                return innerlinks.ToArray();
            }

        }
    }
}
