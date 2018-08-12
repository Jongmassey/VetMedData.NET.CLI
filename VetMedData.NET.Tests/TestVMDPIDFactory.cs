using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VetMedData.NET;
using VetMedData.NET.Util;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestVMDPIDFactory
    {
        [TestMethod]
        public void TestGetPID()
        {
            var pid = VMDPIDFactory.GetVmdpid().Result;
            Assert.IsNotNull(pid, "returned PID object is null");
            Assert.IsNotNull(pid.CreatedDateTime, "pid.CreatedDateTime is null");
            Assert.IsTrue(pid.CurrentlyAuthorisedProducts.Count > 0, "No currently authorised products");
            Assert.IsTrue(pid.ExpiredProducts.Count > 0, "No expired products");
            Assert.IsTrue(pid.SuspendedProducts.Count > 0, "No suspended products");
            Assert.IsTrue(pid.HomoeopathicProducts.Count > 0, "No homoepathic products");
        }

        [TestMethod]
        public void TestAllProducts()
        {
            var pid = VMDPIDFactory.GetVmdpid().Result;
            var ap = pid.AllProducts;

            //check product count
            var totalcount = pid.HomoeopathicProducts.Count
                             + pid.CurrentlyAuthorisedProducts.Count
                             + pid.ExpiredProducts.Count
                             + pid.SuspendedProducts.Count;
            Assert.IsTrue(totalcount == ap.Count(), "Mismatched total counts");

            //check properties of returned products
            foreach (var product in ap)
            {
                //centralised-authorisation products have lots of properties missing
                if (product.AuthorisationRoute.Equals("Centralised")){continue;}

                //check all string properties populated
                foreach (var property in product.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(string)
                                && !p.Name.Equals("UKPAR_Link")
                                && !p.Name.Equals("PAAR_Link")))
                {
                    Assert.IsFalse(string.IsNullOrEmpty(property.GetValue(product).ToString()), $"Property {property.Name} empty for product {product}");
                }

                //check all string list props populated
                foreach (var property in typeof(Product).GetProperties().Where(p => p.PropertyType == typeof(IEnumerable<string>)))
                {
                    Assert.IsTrue(((IEnumerable<string>)property.GetValue(product)).Any(), $"Property {property.Name} empty for product {product}");
                }
            }
        }

        [TestMethod]
        public void TestProductPropertyAggregators()
        {
            var pid = VMDPIDFactory.GetVmdpid().Result;
            foreach (var prop in typeof(VMDPID).GetProperties().Where(p=>p.PropertyType == typeof(IEnumerable<string>)))
            {
                var p = (IEnumerable<string>) prop.GetValue(pid);
                Assert.IsNotNull(p,$"{prop.Name} is null");
                Assert.IsTrue(p.Any(),$"{prop.Name} empty");
            }
            
        }

        [TestMethod]
        public void TestGetPIDWithExpiredProductTargetSpecies()
        {
            var pid = VMDPIDFactory.GetVmdpid(false, true).Result;
            Assert.IsFalse(pid.ExpiredProducts.Where(ep => ep.SPC_Link.ToLower().EndsWith(".doc") ||
                                                           ep.SPC_Link.ToLower().EndsWith(".docx")).Any(ep=>!ep.TargetSpecies.Any()));
        }
    }
}
