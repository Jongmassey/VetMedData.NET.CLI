using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using VetMedData.NET.Model;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestTargetSpecies
    {
        [TestMethod]
        public void TestFind()
        {
            var results = new[]
            {
                "Horses",
                "Dogs",
                "Cats"
            }.Select(s => new {instr = s,res =  TargetSpecies.Find(s)}).ToArray();

            foreach (var result in results)
            {
                Assert.IsNotNull(result.res, $"species {result.instr} not found");
                Assert.IsTrue(result.res.Any(), $"species {result.instr} not found");
            }

            var rescount = results.SelectMany(re => re.res).Distinct().Count();
            Assert.IsTrue(rescount==3,$"Expected 3 results, got {rescount}");
        }
    }
}

