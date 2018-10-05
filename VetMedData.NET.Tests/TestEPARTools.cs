using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            var testset = new Dictionary<string, string>
            {
                {
                    "dicural 150 mg coated tablets for dogs",
                    "https://www.ema.europa.eu/documents/product-information/dicural-epar-product-information_en.pdf"
                },
                {
                    "metacam",
                    "https://www.ema.europa.eu/documents/product-information/metacam-epar-product-information_en.pdf"
                },
                {
                    "purevax rcch",
                    "https://www.ema.europa.eu/documents/product-information/purevax-rcch-epar-product-information_en.pdf"
                },
                {"aivlosin 8.5 mg/g premix for medicated feeding stuff for pigs",
                    "https://www.ema.europa.eu/documents/product-information/aivlosin-epar-product-information_en.pdf"
                },
                {
                    "promeris duo 499.5 mg + 499.5 mg spot-on for medium/large sized dogs",
                    "https://www.ema.europa.eu/documents/product-information/promeris-duo-epar-product-information_en.pdf"
                }

            };
            var testout = new Dictionary<string, string>();

            foreach (var test in testset)
            {
                var res = EPARTools.GetSearchResults(test.Key).Result;

                if (res == null)
                {
                    testout[test.Key] = null;
                }
                else
                if (res.Length==0)
                {
                    testout[test.Key] = string.Empty;
                }
                else
                if (res[0].Equals(test.Value))
                {
                    testout[test.Key] = true.ToString();
                }
                else
                {
                    testout[test.Key] = false.ToString();
                }

                if (!testout[test.Key].Equals(true.ToString()))
                {
                    Debug.WriteLine($"{test.Key}: {testout[test.Key]}");
                }
            }
            
            Assert.IsTrue(testout.Values.All(t=>t.Equals(true.ToString())));

            //var res = EPARTools.GetSearchResults("metacam").Result;
            //Assert.IsNotNull(res,"No results returned");
            //Assert.IsTrue(res.Length>0, "Empty results returned");
            //Assert.IsTrue(res[0].Equals("https://www.ema.europa.eu/documents/product-information/metacam-epar-product-information_en.pdf")
            //    , $"Wrong url returned: {res[0]}");
        }
        
    }
}
