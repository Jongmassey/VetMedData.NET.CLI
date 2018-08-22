using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VetMedData.NET;
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
                "Horses","ponies","donkies and foals over four weeks of age."
            };
            
            var ts = SPCParser.GetTargetSpecies(pathtospc);
            var intersectioncount = ts.Intersect(expectedoutput).Count();
            Assert.IsTrue(intersectioncount == expectedoutput.Length, $"Intersection count:{intersectioncount}, expected 3");
        }

        [TestMethod, DeploymentItem(@"TestFiles\TestSPCParser\", @"TestFiles\TestSPCParser\")]
        public void TestGetPdfPlainText()
        {
            const string pathtopdf = @"TestFiles\TestSPCParser\WC500067567.pdf";

            Assert.IsTrue(File.Exists(pathtopdf), "test file not readable");
            var pt = SPCParser.GetPlainText(pathtopdf);
            Assert.IsFalse(string.IsNullOrWhiteSpace(pt),"Plain text null or whitespace");
        }
    }
}
