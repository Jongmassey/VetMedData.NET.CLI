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
            var input = new[] { "Horses", "Dogs", "Cats" };
            foreach (var s in input)
            {
                Assert.IsNotNull(TargetSpecies.Find(s), $"species {s} not found");
                Assert.IsTrue(TargetSpecies.Find(s).Any(), $"species {s} not found");
            }

        }
    }
}

