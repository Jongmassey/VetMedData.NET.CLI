using System;
using System.IO;
using Spire.Doc;
namespace VetMedData.NET
{
    public static class WordConverter
    {
        public static string ConvertDocToDocx(string pathToDoc)
        {
            var d = new Document();
            var outpath = pathToDoc.Replace(".doc", ".docx",StringComparison.InvariantCultureIgnoreCase);
            d.LoadFromFile(pathToDoc,FileFormat.Doc);
            d.SaveToFile(outpath,FileFormat.Docx);
            return outpath;
        }

        public static Stream ConvertDocToDocx(Stream docStream)
        {
            var d = new Document();
            d.LoadFromStream(docStream,FileFormat.Doc);
            var ms = new MemoryStream();
            d.SaveToStream(ms,FileFormat.Docx);
            return ms;
        }
    }
}
