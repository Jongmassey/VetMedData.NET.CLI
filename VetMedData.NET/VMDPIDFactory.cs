using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace VetMedData.NET
{
    public class VMDPIDFactory
    {
        private const string VmdUrl = @"http://www.vmd.defra.gov.uk/ProductInformationDatabase/downloads/VMD_ProductInformationDatabase.xml";
        private const string fmt = @"dd/MM/yyyy HH:mm:ss";

        private static readonly HttpClient Client = new HttpClient();
        private static readonly XmlSerializer Xser = new XmlSerializer(typeof(VMDPID_Raw), new XmlRootAttribute("VMD_PIDProducts"));
        private static VMDPID _vmdpid;

        //convert autogen-from-xsd class hierarchy into clean human-created one
        private static VMDPID CleanAndParse(VMDPID_Raw raw, DateTime? createdDateTime)
        {

            var output = new VMDPID();
            if (createdDateTime.HasValue)
                { output.CreatedDateTime = createdDateTime.Value; }

            foreach (var rcp in raw.CurrentAuthorisedProducts)
            {
                var ocp = new CurrentlyAuthorisedProduct
                {
                    ActiveSubstances = rcp.ActiveSubstances.Split(',').Select(a=>a.Trim()),
                    AuthorisationRoute = rcp.AuthorisationRoute.Trim(),
                    ControlledDrug =  rcp.ControlledDrug,
                    DateOfIssue = rcp.DateOfIssue,
                    DistributionCategory = rcp.DistributionCategory.Trim(),
                    Distributors = rcp.Distributors.Replace("&lt;span&gt","").Split(';').Select(a => a.Trim()),
                    MAHolder = rcp.MAHolder.Trim(),
                    Name =  rcp.Name.Trim(),
                    PAAR_Link = rcp.PAAR_Link.Trim(),
                    PharmaceuticalForm = rcp.PharmaceuticalForm.Trim(),
                    SPC_Link = rcp.SPC_Link.Trim(),
                    TargetSpecies = rcp.TargetSpecies.Split(',').Select(a => a.Trim()),
                    TherapeuticGroup = rcp.TherapeuticGroup.Trim(),
                    UKPAR_Link = rcp.UKPAR_Link.Trim(),
                    VMNo = rcp.VMNo.Trim()

                };
                output.CurrentlyAuthorisedProducts.Add(ocp);

            }

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

        public static async Task<VMDPID> GetVmdpid()
        {
            if(_vmdpid == null) { 

            //load incoming stream from HTTP as LINQ to XML element
            var xe = XDocument.Load(await GetXMLStream());
            var comments = xe.DescendantNodes().OfType<XComment>();
            //extract datetime from first comment that ends with a valid dt
            DateTime dt = default(DateTime);
            
            foreach (var comment in comments)
            {
                if (DateTime.TryParseExact(comment.Value.Substring(comment.Value.Length - fmt.Length)
                    , fmt, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
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
