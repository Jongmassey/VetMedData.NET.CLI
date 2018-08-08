using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VetMedData.NET;

namespace VetMedData.Tests
{
    [TestClass]
    public class TestWordConverter
    {
        [TestMethod]
        public void TestDocToDocxFile()
        {
            //todo:move project folders
            var docpath = "C:\\temp\\wordconvertertest\\SPC_91079.DOC";
            var docxpath = WordConverter.ConvertDocToDocx(docpath);
            Assert.IsTrue(File.Exists(docxpath));

        }
    }
}
