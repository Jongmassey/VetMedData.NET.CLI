using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VetMedData.NET.Model;
using VetMedData.NET.Util;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestVMDPIDFactory
    {
        private readonly string[] _missingTherapeuticGroup =
        {
            //"eu/2/16/197/001",
            //"eu/2/17/209/001–002",
            //"eu/2/17/220/001",
            //"eu/2/17/220/003",
            //"eu/2/17/220/004",
            //"eu/2/17/220/005",
            //"eu/2/17/220/006",

        };

        private readonly string[] _missingTargetSpecies =
        {
            //"eu/2/15/191/004-006",
            //"eu/2/15/191/007-009",
            //"eu/2/15/191/010-012",
            //"eu/2/15/191/013-015",
            //"eu/2/15/191/016-018",
        };

        private readonly string[] _missingDistributionCategory =
        {
            //"eu/2/16/202/001-003",
            //"eu/2/17/217/001-002",
        };


        [TestMethod]
        public void TestGetPID()
        {
            var pid = VMDPIDFactory.GetVmdPid().Result;
            Assert.IsNotNull(pid, "returned PID object is null");
            Assert.IsNotNull(pid.CreatedDateTime, "pid.CreatedDateTime is null");
            Assert.IsTrue(pid.CurrentlyAuthorisedProducts.Count > 0, "No currently authorised products");
            Assert.IsTrue(pid.ExpiredProducts.Count > 0, "No expired products");
            Assert.IsTrue(pid.SuspendedProducts.Count > 0, "No suspended products");
            Assert.IsTrue(pid.HomoeopathicProducts.Count > 0, "No homoeopathic products");
        }

        [TestMethod]
        public void TestAllProducts()
        {
            var pid = VMDPIDFactory.GetVmdPid().Result;
            var ap = pid.AllProducts;

            //check product count
            var totalCount = pid.HomoeopathicProducts.Count
                             + pid.CurrentlyAuthorisedProducts.Count
                             + pid.ExpiredProducts.Count
                             + pid.SuspendedProducts.Count;
            Assert.IsTrue(totalCount == ap.Count(), "Mismatched total counts");

            //check properties of returned products
            foreach (var product in ap)
            {
                //centralised-authorisation products have lots of properties missing
                if (product.AuthorisationRoute.Equals("Centralised")) { continue; }

                //check all string properties populated
                foreach (var property in product.GetType().GetProperties()
                    .Where(p => p.PropertyType == typeof(string)
                                && !p.Name.Equals("UKPAR_Link")
                                && !p.Name.Equals("PAAR_Link")))
                {
                    if (_missingTherapeuticGroup.Contains(product.VMNo) && property.Name.Equals("TherapeuticGroup")) { continue; }
                    if (_missingDistributionCategory.Contains(product.VMNo) && property.Name.Equals("DistributionCategory")) { continue; }

                    Assert.IsFalse(string.IsNullOrEmpty(property.GetValue(product).ToString()), $"Property {property.Name} empty for product {product}");
                }

                //check all string list props populated
                foreach (var property in typeof(ReferenceProduct).GetProperties()
                    .Where(p => p.PropertyType == typeof(IEnumerable<string>)))
                {
                    if ((typeof(ExpiredProduct) == product.GetType() || 
                         _missingTargetSpecies.Contains(product.VMNo))
                        && property.Name.Equals("TargetSpecies"))
                    { continue; }

                    Assert.IsTrue(((IEnumerable<string>)property.GetValue(product)).Any(), $"Property {property.Name} empty for product {product}");
                }
            }
        }

        [TestMethod]
        public void TestProductPropertyAggregators()
        {
            var pid = VMDPIDFactory.GetVmdPid().Result;
            foreach (var prop in typeof(VMDPID).GetProperties().Where(p => p.PropertyType == typeof(IEnumerable<string>)))
            {
                var p = (IEnumerable<string>)prop.GetValue(pid);
                Assert.IsNotNull(p, $"{prop.Name} is null");
                Assert.IsTrue(p.Any(), $"{prop.Name} empty");
            }

        }

        [TestMethod]
        public void TestTargetSpeciesExtractionLocal()
        {
            var pid = VMDPIDFactory.GetVmdPid().Result;
            foreach (var ep in pid.ExpiredProducts.Where(ep => !EPARTools.IsEPAR(ep.SPC_Link)))
            {
                var spc = VMDPIDFactory.GetSpc(ep).Result;
                Debug.WriteLine(spc);
                spc = spc.ToLowerInvariant().EndsWith(".doc") ? WordConverter.ConvertDocToDocx(spc) : spc;
                var ts = SPCParser.GetTargetSpecies(spc);
                Assert.IsNotNull(ts, $"null ts for {ep.Name}, {spc}");
                Assert.IsTrue(ts.Any(), $"empty ts for {ep.Name}, {spc}");
                Assert.IsFalse(ts.Any(string.IsNullOrWhiteSpace), $"blank species for {ep.Name}, {spc}");
            }
        }

        [TestMethod]
        public void TestStaticTypingOfTargetSpecies()
        {
            var pid = VMDPIDFactory.GetVmdPid().Result;
            foreach (var p in pid.AllProducts)
            {
                if (p.GetType() == typeof(ExpiredProduct) || _missingTargetSpecies.Contains(p.VMNo)) continue;
                Assert.IsTrue(p.TargetSpeciesTyped != null && p.TargetSpeciesTyped.Any(),
                    $"Non-expired with un-filled strongly typed target species for {p.Name}");
            }
        }

        [TestMethod]
        public void TestStaticTypingOfTargetSpeciesExpiredProducts()
        {
            var pid = VMDPIDFactory.GetVmdPid(PidFactoryOptions.GetTargetSpeciesForExpiredVmdProduct).Result;
            foreach (var p in pid.AllProducts)
            {
                if (p.GetType() != typeof(ExpiredProduct)
                    || EPARTools.IsEPAR(((ExpiredProduct)p).SPC_Link)
                    ) continue;
                if (!(p.TargetSpeciesTyped != null && p.TargetSpeciesTyped.Any()))
                {
                    Debug.WriteLine(string.Join(';', p.TargetSpecies));
                }
                Assert.IsTrue(p.TargetSpeciesTyped != null && p.TargetSpeciesTyped.Any(),
                    $"Expired product with un-filled strongly typed target species for {p.Name}"
                    + $"from {string.Join(';', p.TargetSpecies)}");
            }
        }

        [TestMethod]
        public void TestStaticTypingOfTargetSpeciesEmaLicensedExpiredProducts()
        {
            var pid = VMDPIDFactory.GetVmdPid(PidFactoryOptions.GetTargetSpeciesForExpiredEmaProduct).Result;
            var errorString = "";
            var errors = pid.AllProducts
                .Where(p => p.GetType() == typeof(ExpiredProduct) && EPARTools.IsEPAR(((ExpiredProduct) p).SPC_Link))
                .Where(p => p.TargetSpeciesTyped == null || !p.TargetSpeciesTyped.Any()).ToList();
            
            if (errors.Any())
            {
                errorString = string.Join(Environment.NewLine,
                    errors.Select(p => $"{p.Name} " +
                                       $"from {string.Join(';', p.TargetSpecies??new string[0])}"));
            }

            Assert.IsFalse(errors.Any(),
                $"EMA-licensed expired product with un-filled strongly typed target species for: {errorString}");
        }

        [TestMethod]
        public void TestGetPIDWithExpiredProductTargetSpecies()
        {
            var pid = VMDPIDFactory.GetVmdPid(PidFactoryOptions.GetTargetSpeciesForExpiredVmdProduct).Result;
            Assert.IsFalse(pid.ExpiredProducts
                                                .Where(ep => ep.SPC_Link.ToLower().EndsWith(".doc") ||
                                                           ep.SPC_Link.ToLower().EndsWith(".docx"))
                                                .Any(ep => !ep.TargetSpecies.Any()));
        }

        [TestMethod]
        public void TestGetPIDWithEuropeanExpiredProductTargetSpecies()
        {
            var pid = VMDPIDFactory.GetVmdPid(PidFactoryOptions.GetTargetSpeciesForExpiredEmaProduct).Result;
            foreach (var missingProduct in pid.ExpiredProducts
                .Where(ep => ep.SPC_Link.ToLowerInvariant()
                    .Contains("ema.europa.eu"))
                .Where(ep => ep.TargetSpecies == null ||
                    !ep.TargetSpecies.Any()))
            {
                Debug.WriteLine(missingProduct.Name);
            }

            Assert.IsFalse(pid.ExpiredProducts
                .Where(ep => ep.SPC_Link.ToLowerInvariant()
                    .Contains("ema.europa.eu"))
                .Any(ep => ep.TargetSpecies == null ||
                           !ep.TargetSpecies.Any()));

            foreach (var expiredProduct in pid.ExpiredProducts
                .Where(ep => ep.SPC_Link.ToLowerInvariant()
                    .Contains("ema.europa.eu")))
            {
                foreach (var sp in expiredProduct.TargetSpecies)
                {
                    Debug.WriteLine($"{expiredProduct.Name}\t{sp}");
                }
            }

        }

       }
}
