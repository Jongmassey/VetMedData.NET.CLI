using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using VetMedData.NET.Model;

namespace VetMedData.NET.Util
{
    /// <summary>
    /// Factory class for VMDPID, handles GETting and parsing of XML.
    /// Has singleton-like behaviour for VMDPID class as to reduce
    /// number of HTTP GETs to VMD servers.
    /// </summary>

    // ReSharper disable once InconsistentNaming
    public static class VMDPIDFactory
    {
        private const string VmdUrl = @"http://www.vmd.defra.gov.uk/ProductInformationDatabase/downloads/VMD_ProductInformationDatabase.xml";
        private const string DateTimeFormat = @"dd/MM/yyyy HH:mm:ss";

        private static readonly HttpClient Client = new HttpClient();
        private static readonly XmlSerializer Xser = new XmlSerializer(typeof(VMDPID_Raw), new XmlRootAttribute("VMD_PIDProducts"));
        private static VMDPID _vmdpid;


        /// <summary>
        /// Converts autogen-from-xsd (VMD_PIDProducts.xsd)
        /// class hierarchy into clean human-created one
        /// </summary>
        /// <param name="raw">As-parsed VMDPID_Raw</param>
        /// <param name="createdDateTime">Created DateTime from xml comment</param>
        /// <returns>Instance of VMDPID containing cleaned data from raw</returns>
        private static VMDPID CleanAndParse(VMDPID_Raw raw, DateTime? createdDateTime)
        {

            var output = new VMDPID
            {
                //TODO: refactor repetitive product processing logic
                CurrentlyAuthorisedProducts = raw.CurrentAuthorisedProducts.Select(rcp =>
                    new CurrentlyAuthorisedProduct
                    {
                        ActiveSubstances = rcp.ActiveSubstances.Split(',').Select(a => a.Trim()),
                        AuthorisationRoute = rcp.AuthorisationRoute.Trim(),
                        ControlledDrug = rcp.ControlledDrug,
                        DateOfIssue = rcp.DateOfIssue,
                        DistributionCategory = rcp.DistributionCategory.Trim(),
                        //remove stray html tags in Distributor field
                        Distributors = rcp.Distributors.Replace("&lt;span&gt", "").Split(';').Select(a => a.Trim()),
                        MAHolder = rcp.MAHolder.Trim(),
                        Name = rcp.Name.Trim(),
                        PAAR_Link = rcp.PAAR_Link.Trim(),
                        PharmaceuticalForm = rcp.PharmaceuticalForm.Trim(),
                        SPC_Link = rcp.SPC_Link.Trim(),
                        TargetSpecies = rcp.TargetSpecies.Split(',').Select(a => a.Trim()),
                        TherapeuticGroup = rcp.TherapeuticGroup.Trim(),
                        UKPAR_Link = rcp.UKPAR_Link.Trim(),
                        VMNo = rcp.VMNo.Trim()

                    }).ToList(),
                ExpiredProducts = raw.ExpiredProducts.Select(rep =>
                    new ExpiredProduct
                    {
                        ActiveSubstances = rep.ActiveSubstances.Split(',').Select(a => a.Trim()),
                        AuthorisationRoute = rep.AuthorisationRoute.Trim(),
                        MAHolder = rep.MAHolder.Trim(),
                        Name = rep.Name.Trim(),
                        SPC_Link = rep.SPC_Link.Trim(),
                        VMNo = rep.VMNo.Trim(),
                        DateofExpiration = rep.DateOfExpiration
                    }).ToList(),
                SuspendedProducts = raw.SuspendedProducts.Select(rsp =>
                    new SuspendedProduct()
                    {
                        ActiveSubstances = rsp.ActiveSubstances.Split(',').Select(a => a.Trim()),
                        AuthorisationRoute = rsp.AuthorisationRoute.Trim(),
                        ControlledDrug = rsp.ControlledDrug,
                        DateOfIssue = rsp.DateOfIssue,
                        DistributionCategory = rsp.DistributionCategory.Trim(),
                        MAHolder = rsp.MAHolder.Trim(),
                        Name = rsp.Name.Trim(),
                        PAAR_Link = rsp.PAAR_Link.Trim(),
                        PharmaceuticalForm = rsp.PharmaceuticalForm.Trim(),
                        SPC_Link = rsp.SPC_Link.Trim(),
                        TargetSpecies = rsp.TargetSpecies.Split(',').Select(a => a.Trim()),
                        TherapeuticGroup = rsp.TherapeuticGroup.Trim(),
                        UKPAR_Link = rsp.UKPAR_Link.Trim(),
                        VMNo = rsp.VMNo.Trim(),
                        DateOfSuspension = rsp.DateOfSuspension

                    }).ToList(),
                HomoeopathicProducts = raw.HomeopathicProducts.Select(rhp =>
                    new HomoeopathicProduct
                    {
                        ActiveSubstances = rhp.ActiveSubstances.Split(',').Select(a => a.Trim()),
                        AuthorisationRoute = rhp.AuthorisationRoute.Trim(),
                        ControlledDrug = rhp.ControlledDrug,
                        DateOfIssue = rhp.DateOfIssue,
                        DistributionCategory = rhp.DistributionCategory.Trim(),
                        MAHolder = rhp.MAHolder.Trim(),
                        Name = rhp.Name.Trim(),
                        PharmaceuticalForm = rhp.PharmaceuticalForm.Trim(),
                        TargetSpecies = rhp.TargetSpecies.Split(',').Select(a => a.Trim()),
                        TherapeuticGroup = rhp.TherapeuticGroup.Trim(),
                        VMNo = rhp.VMNo.Trim()
                    }).ToList(),
                CreatedDateTime = createdDateTime ?? default(DateTime)

            };

            return output;
        }

        /// <summary>
        /// HTTP GETs XML PID from VMD as stream
        /// </summary>
        /// <returns></returns>
        private static async Task<Stream> GetHttpStream(string url)
        {
            var ms = new MemoryStream();
            using (var resp = await Client.GetAsync(url))
            using (var instream = await resp.Content.ReadAsStreamAsync())
            {
                await instream.CopyToAsync(ms);
            }

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        private static async Task<VMDPID> PopulateExpiredProductTargetSpecies(VMDPID inpid)
        {
            foreach (var expiredProduct in inpid.ExpiredProducts.Where(ep => ep.SPC_Link.ToLower().EndsWith(".doc") ||
                                                                             ep.SPC_Link.ToLower().EndsWith(".docx")))
            {
                var tdoc = "";
                // ReSharper disable once RedundantAssignment
                var tdocx = "";
                // ReSharper disable once RedundantAssignment
                var tf = "";
                var doc = expiredProduct.SPC_Link.EndsWith(".doc", StringComparison.InvariantCultureIgnoreCase);
                if (doc)
                {
                    tf = Path.GetTempFileName();
                    tdoc = $"{tf}.doc";
                    File.Move(tf, tdoc);

                    using (var fs = File.OpenWrite(tdoc))
                    {
                        (await GetHttpStream(expiredProduct.SPC_Link)).CopyTo(fs);
                        fs.Flush();
                    }

                    tdocx = WordConverter.ConvertDocToDocx(tdoc);
                }
                else
                {
                    tf = Path.GetTempFileName();
                    tdocx = $"{tf}.docx";
                    File.Move(tf, tdocx);
                    using (var fs = File.OpenWrite(tdocx))
                    {
                        (await GetHttpStream(expiredProduct.SPC_Link)).CopyTo(fs);
                    }
                }

                using (var spcStream = File.Open(tdocx, FileMode.Open))
                {
                    var ts = SPCParser.GetTargetSpecies(spcStream);
                    expiredProduct.TargetSpecies = ts;
                }

                if (!string.IsNullOrEmpty(tdoc) && File.Exists(tdoc))
                {
                    File.Delete(tdoc);
                }

                if (!string.IsNullOrEmpty(tdocx) && File.Exists(tdocx))
                {
                    File.Delete(tdocx);
                }

            }
            return inpid;
        }

        private static async Task<VMDPID> PopulateExpiredProductTargetSpeciesFromEMA(VMDPID inpid)
        {

            foreach (var expiredProduct in inpid.ExpiredProducts.Where(ep => EPARTools.IsEPAR(ep.SPC_Link)))
            {
                var possibleTargetSpecies = new Dictionary<string, string[]>();
                var searchResults = await EPARTools.GetSearchResults(expiredProduct.Name);
                foreach (var result in searchResults)
                {
                    var tf = Path.GetTempFileName();
                    using (var tfs = File.OpenWrite(tf))
                    {
                        (await GetHttpStream(result)).CopyTo(tfs);
                    }

                    var targetSpecies = SPCParser.GetTargetSpeciesFromMultiProductPdf(tf);
                    foreach (var ts in targetSpecies)
                    {
                        possibleTargetSpecies[ts.Key] = ts.Value;
                    }
                    File.Delete(tf);
                }
                if (possibleTargetSpecies.ContainsKey(expiredProduct.Name))
                {
                    expiredProduct.TargetSpecies = possibleTargetSpecies[expiredProduct.Name];
                }
                else
                {
                    Debug.Write($"{expiredProduct.Name} Product not found");
                }
            }

            return inpid;
        }


        /// <summary>
        /// Gets copy of VMDPID.
        /// If copy has already been downloaded, and overrideStoredInstance set to
        /// False (default) then cached copy will be returned.
        /// </summary>
        /// <param name="overrideStoredInstance">
        /// Setting to true will cause new copy of XML PID to be downloaded from VMD
        /// </param>
        /// <param name="getTargetSpeciesForExpiredProducts">
        /// Setting to true will HTTP get the SPC document for every expired product and
        /// will attempt to parse the TargetSpecies section into structured data.
        /// </param>
        /// <param name="getTargetSpeciesForEuropeanExpiredProducts">
        /// Setting to true will attempt to search for the product on the EMA website,
        /// scrape the results and navigate to the pdf download link for the SPC. This
        /// pdf file will be parsed to find the TargetSpecies section.</param>
        /// <returns></returns>
        public static async Task<VMDPID> GetVmdpid(bool overrideStoredInstance = false,
            bool getTargetSpeciesForExpiredProducts = false,
            bool getTargetSpeciesForEuropeanExpiredProducts = false)
        {
            if (overrideStoredInstance || _vmdpid == null)
            {
                //load incoming stream from HTTP as LINQ to XML element
                var xe = XDocument.Load(await GetHttpStream(VmdUrl));
                var comments = xe.DescendantNodes().OfType<XComment>();

                //get first comment which ends in a valid datetime as per DateTimeFormat
                var dt = default(DateTime);
                foreach (var comment in comments)
                {
                    if (DateTime.TryParseExact(comment.Value.Substring(comment.Value.Length - DateTimeFormat.Length)
                        , DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                    {
                        break;
                    }
                }

                var raw = (VMDPID_Raw)Xser.Deserialize(xe.CreateReader());
                _vmdpid = CleanAndParse(raw, dt == default(DateTime) ? (DateTime?)null : dt);
            }

            if (getTargetSpeciesForExpiredProducts)
            {
                _vmdpid = await PopulateExpiredProductTargetSpecies(_vmdpid);
            }

            if (getTargetSpeciesForEuropeanExpiredProducts)
            {
                _vmdpid = await PopulateExpiredProductTargetSpeciesFromEMA(_vmdpid);
            }

            return _vmdpid;
            //return getTargetSpeciesForExpiredProducts
            //    ? await PopulateExpiredProductTargetSpecies(
            //        getTargetSpeciesForEuropeanExpiredProducts ?
            //            await PopulateExpiredProductTargetSpeciesFromEMA(_vmdpid) :
            //            _vmdpid)
            //    : _vmdpid;
        }

    }
}
