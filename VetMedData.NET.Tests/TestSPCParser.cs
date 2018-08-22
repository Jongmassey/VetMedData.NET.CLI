using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using VetMedData.NET.Util;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestSPCParser
    {
        [TestMethod, DeploymentItem(@"TestFiles\TestSPCParser\", @"TestFiles\TestSPCParser\")]
        public void TestTargetSpeciesExtraction()
        {
            const string pathtospc = @"TestFiles\TestSPCParser\SPC_91079.docx";

            var expectedoutput = new[]
            {
                "horses","ponies","donkies","foals over four weeks of age"
            };

            var ts = SPCParser.GetTargetSpecies(pathtospc);
            var intersectioncount = ts.Intersect(expectedoutput).Count();
            Assert.IsTrue(intersectioncount == expectedoutput.Length,
                $"Intersection count:{intersectioncount}, expected {expectedoutput.Length}");
        }

        [TestMethod, DeploymentItem(@"TestFiles\TestSPCParser\", @"TestFiles\TestSPCParser\")]
        public void TestGetPdfPlainText()
        {
            const string pathtopdf = @"TestFiles\TestSPCParser\WC500067567.pdf";

            var pt = SPCParser.GetPlainText(pathtopdf);
            Assert.IsFalse(string.IsNullOrWhiteSpace(pt), "Plain text null or whitespace");
        }

        [TestMethod, DeploymentItem(@"TestFiles\TestSPCParser\", @"TestFiles\TestSPCParser\")]
        public void TestGetTargetSpeciesFromPDF()
        {
            const string pathtopdf = @"TestFiles\TestSPCParser\WC500067567.pdf";
            var sp = SPCParser.GetTargetSpeciesFromPdf(pathtopdf);
            Assert.IsNotNull(sp, "Nothing returned");
            Assert.IsTrue(sp.Length == 1, $"Returned {sp.Length} species instead of 1");
            Assert.IsTrue(sp[0].Equals("horse", StringComparison.InvariantCultureIgnoreCase), $"returned: {sp[0]} instead of horse");
        }

        [TestMethod, DeploymentItem(@"TestFiles\TestSPCParser\", @"TestFiles\TestSPCParser\")]
        public void TestGetTargetSpeciesFromMultiPDF()
        {
            const string pathtopdf = @"TestFiles\TestSPCParser\WC500065777.pdf";
            var sp = SPCParser.GetTargetSpeciesFromMultiProductPdf(pathtopdf);
            Assert.IsNotNull(sp, "null dictionary returned");
            Assert.IsFalse(sp.Count == 0, "empty dictionary returned");
            Assert.IsTrue(sp.ContainsKey("Metacam 20 mg/ml solution for injection for cattle, pigs and horses"), "product not found");
            Assert.IsTrue(
                sp["Metacam 20 mg/ml solution for injection for cattle, pigs and horses"]
                    .Intersect(new[] { "cattle", "pigs", "horses" }).Count() == 3,
                $"Unexpected species list returned:{string.Join(',', sp["Metacam 20 mg/ml solution for injection for cattle, pigs and horses"])}");
        }
    }
}
