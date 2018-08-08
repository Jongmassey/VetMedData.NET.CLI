using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VetMedData.NET;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestSPCParser
    {
        [TestMethod, DeploymentItem(@"TestFiles\TestSPCParser\", @"TestFiles\TestSPCParser\")]
        public void TestTargetSpeciesExtraction()
        {
            //todo:refer to file within project
            const string pathtospc = @"TestFiles\TestSPCParser\SPC_91079.docx";

            var expectedoutput = new[]
            {
                "Horses","ponies","donkies and foals over four weeks of age."
            };
            
            var ts = SPCParser.GetTargetSpecies(pathtospc);
            var intersectioncount = ts.Intersect(expectedoutput).Count();
            Assert.IsTrue(intersectioncount == expectedoutput.Length, $"Intersection count:{intersectioncount}, expected 3");
        }


    }
}
