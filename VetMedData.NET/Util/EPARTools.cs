using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        private const string EmaSearchUrl =
            @"https://www.ema.europa.eu/medicines/veterinary/EPAR/{prodname}";


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
        /// Builds link to the product page then tries to get, returns empty array if not found.
        /// Then gets product page, xpath's links and
        /// filters those to get english-language PDF of SPC document.
        /// </summary>
        /// <param name="productName">ReferenceProduct Name to search for</param>
        /// <returns>Array of english-language SPC URLs matching product name</returns>
        private static async Task<string[]> GetSearchResultsInternal(string productName)
        {
            using (var cli = new HttpClient())
            {
                //build search URL
                //cli.BaseAddress = EmaBaseUri;
                var innerlinks = new List<string>();

                var res = await cli.GetAsync(EmaSearchUrl.Replace("{prodname}", productName));
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    return innerlinks.ToArray();
                }

                //load results page
                var doc = new HtmlDocument();
                doc.Load(await res.Content.ReadAsStreamAsync());

                //extract list of links

                var doclinks = doc.DocumentNode.SelectNodes("//a[@href]")
                    .Where(n =>
                        n.InnerText.Contains("EPAR - Product Information") &&
                        n.Attributes["href"].Value.EndsWith(".pdf"))
                    .Select(n => n.Attributes["href"].Value);
                innerlinks.AddRange(doclinks);

                return innerlinks.ToArray();
            }

        }
    }
}
