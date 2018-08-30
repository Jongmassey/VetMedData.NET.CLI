using Microsoft.VisualStudio.TestTools.UnitTesting;
using VetMedData.NET.Util;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestEPARTools
    {
        [TestMethod]
        public void TestGetSearchResults()
        {
            var res = EPARTools.GetSearchResults("metacam").Result;
            Assert.IsNotNull(res,"No results returned");
            Assert.IsTrue(res.Length>0, "Empty results returned");
            Assert.IsTrue(res[0].Equals("http://www.ema.europa.eu/docs/en_GB/document_library/EPAR_-_Product_Information/veterinary/000033/WC500065777.pdf")
                ,$"Wrong url returned: {res[0]}");
        }
    }
}
