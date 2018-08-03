﻿using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace VetMedData.NET
{
    /// <summary>
    /// Factory class for VMDPID, handles GETting and parsing of XML.
    /// Has singleton-like behaviour for VMDPID class as to reduce
    /// number of HTTP GETs to VMD servers.
    /// </summary>
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
                CurrentlyAuthorisedProducts = raw.CurrentAuthorisedProducts.Select(rcp=>
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
                ExpiredProducts = raw.ExpiredProducts.Select(rep=>
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
                HomoeopathicProducts = raw.HomeopathicProducts.Select(rhp=>
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

        private static async Task<Stream> GetXMLStream()
        {
            var ms = new MemoryStream();
            using (var resp = await Client.GetAsync(VmdUrl))
            using (var instream = await resp.Content.ReadAsStreamAsync())
            {
                await instream.CopyToAsync(ms);
            }

            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public static async Task<VMDPID> GetVmdpid(bool overrideStoredInstance = false)
        {
            if(overrideStoredInstance || _vmdpid == null) { 

            //load incoming stream from HTTP as LINQ to XML element
            var xe = XDocument.Load(await GetXMLStream());
            var comments = xe.DescendantNodes().OfType<XComment>();
            //extract datetime from first comment that ends with a valid dt
            DateTime dt = default(DateTime);
            
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

            return _vmdpid;
        }

    }
}