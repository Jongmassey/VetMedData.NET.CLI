using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace VetMedData.NET.Util
{
    /// <summary>
    /// Collection of useful methods for working with European Public Assessment Reports
    /// of veterinary medicines from the European Medicines Agency
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class EPARTools
    {
        private static readonly Uri EmaBaseUri = new Uri("http://www.ema.europa.eu/ema/");

        //would be nice to use HttpUtility.ParseQueryString and handle as 
        //NameValueCollection but multiple "status" params get reformatted into
        //comma-separated single param that the EMA search endpoint doesn't accept.
        private const string EmaSearchUrl =
            @"index.jsp?curl=pages%2Fmedicines%2Flanding%2Fvet_epar_search.jsp&mid=WC0b01ac058001fa1c&searchTab=searchByKey&alreadyLoaded=true&isNewQuery=true&status=Authorised&status=Withdrawn&status=Suspended&status=Refused&keyword={prodname}&keywordSearch=Submit&searchType=name&taxonomyPath=&treeNumber=";

        // ReSharper disable once InconsistentNaming
        public static bool IsEPAR(string url)
        {
            return url.Contains(EmaBaseUri.ToString());
        }

        /// <summary>
        /// Searches for SPC document for a product.
        /// Iteratively reduces name by last term until match is found.
        /// </summary>
        /// <param name="productName">ReferenceProduct Name to search for</param>
        /// <returns>Array of english-language SPC URLs matching product name</returns>
        public static async Task<string[]> GetSearchResults(string productName)
        {
            string[] res;

            do
            {
                res = await GetSearchResultsInternal(productName);
                productName = string.Join(' ', productName.Split(' ').Take(productName.Split(' ').Length - 1));
                if (string.IsNullOrWhiteSpace(productName))
                {
                    break;
                }
            } while (res == null || res.Length == 0);

            return res;
        }

        /// <summary>
        /// Searches for product using EPAR product search. If result found
        /// uses an XPath query to get links from document, then filters those to get
        /// the link to the product page. Then gets product page, xpath's links and
        /// filters those to get english-language PDF of SPC document.
        /// </summary>
        /// <param name="productName">ReferenceProduct Name to search for</param>
        /// <returns>Array of english-language SPC URLs matching product name</returns>
        private static async Task<string[]> GetSearchResultsInternal(string productName)
        {
            using (var cli = new HttpClient())
            {
                //build search URL
                cli.BaseAddress = EmaBaseUri;
                var innerlinks = new List<string>();
                var res = await cli.GetAsync(EmaSearchUrl.Replace("{prodname}", productName));

                //load results page
                var doc = new HtmlDocument();
                doc.Load(await res.Content.ReadAsStreamAsync());

                //extract list of links
                var outerlinks = doc.DocumentNode.SelectNodes("//a[@href]");

                //product result (if any) link will be link with same text as search term
                //should only be one but, you never know...
                foreach (var outerlink in outerlinks.Where(n =>
                    n.InnerText.Equals(productName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var productResponse = await cli.GetAsync(outerlink.Attributes["href"].Value);

                    var innerdoc = new HtmlDocument();
                    innerdoc.Load(await productResponse.Content.ReadAsStreamAsync());
                    //extract en_GB pdf links to "ReferenceProduct Information" - i.e. SPC
                    var doclinks = innerdoc.DocumentNode.SelectNodes("//a[@href]")
                        .Where(n =>
                            n.InnerText.Contains("EPAR - Product Information") &&
                            n.Attributes["href"].Value.Contains("en_GB") &&
                            n.Attributes["href"].Value.EndsWith(".pdf"))
                        //format as absolute URI
                        .Select(n => $"http://{EmaBaseUri.Host}{n.Attributes["href"].Value}");

                    innerlinks.AddRange(doclinks);
                }

                return innerlinks.ToArray();
            }

        }
    }
}
