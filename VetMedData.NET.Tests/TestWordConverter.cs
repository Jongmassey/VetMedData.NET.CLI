using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VetMedData.NET;
using VetMedData.NET.Util;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestWordConverter
    {
        [TestMethod, DeploymentItem(@"TestFiles\TestWordConverter\", @"TestFiles\TestWordConverter\")]
        public void TestDocToDocxFile()
        {
            var docpath = @"TestFiles\TestWordConverter\SPC_91079.DOC";
            var docxpath = WordConverter.ConvertDocToDocx(docpath);
            Assert.IsTrue(File.Exists(docxpath));
            File.Delete(docxpath);

        }
    }
}
