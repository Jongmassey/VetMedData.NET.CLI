using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
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
        private const string EmaSearchUrl =
            @"https://www.ema.europa.eu/medicines/veterinary/EPAR/{prodname}";


        // ReSharper disable once InconsistentNaming
        public static bool IsEPAR(string url)
        {
            return url.Contains("ema.europa.eu");
        }

        /// <summary>
        /// Searches for SPC document for a product.
        /// Iteratively reduces name by last term until match is found.
        /// </summary>
        /// <param name="productName">ReferenceProduct Name to search for</param>
        /// <returns>Array of english-language SPC URLs matching product name</returns>
        public static async Task<string[]> GetSearchResults(string productName)
        {
            var outResult = new string[0];
            productName = Regex.Replace(productName, @"[^a-z 1-9]", "", RegexOptions.IgnoreCase);
            for (var i = 1; i <= productName.Split(' ').Length; i++)
            {
                var productSubName = string.Join('-', productName.Split(' ').Take(i));
                var result = await GetSearchResultsInternal(productSubName);
                if (result != null && result.Length > 0)
                {
                    outResult = result;
                }
            }
            return outResult;
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
                        n.Attributes["href"].Value.EndsWith("_en.pdf"))
                    .Select(n => n.Attributes["href"].Value);
                innerlinks.AddRange(doclinks);

                return innerlinks.ToArray();
            }

        }
    }
}
