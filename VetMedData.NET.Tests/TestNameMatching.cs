using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VetMedData.NET;
using VetMedData.NET.ProductMatching;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestNameMatching
    {
        [TestMethod]
        public void TestIdenticalStrings()
        {
            var pnm = new ProductNameMetric();
            string[][] testsStrings = {
                new[] {"TestString", "TestString"},
                new[] {"Test String", "Test String"}
            };
            foreach (var testStringPair in testsStrings)
            {
                var sim = pnm.GetSimilarity(testStringPair[0], testStringPair[1]);
                Assert.IsTrue(Math.Abs(1 - sim) < 0.0001d
                              ,$"Strings: {testStringPair[0]},{testStringPair[1]} returned {sim} instead of 1");
            }
            
        }

        [TestMethod]
        public void TestSingleStringWithKnownSimilarity()
        {
            var pnm = new ProductNameMetric();
            string[] testStringPair = {"teststring", "teststrang"};
            var sim = pnm.GetSimilarity(testStringPair[0], testStringPair[1]);
            Assert.IsTrue(0.9d == sim ,
                $"Strings: {testStringPair[0]},{testStringPair[1]} returned {sim} instead of 1");
        }
    }
}
