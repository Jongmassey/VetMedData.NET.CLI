using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VetMedData.NET.Util;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestEPARTools
    {
        [TestMethod]
        public void TestCtor()
        {
            var tools = EPARTools.Get();
            Assert.IsNotNull(tools);
        }
    }
}
