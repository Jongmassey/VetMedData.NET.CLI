using System;
using System.IO;
using Spire.Doc;

namespace VetMedData.NET.Util
{
    /// <summary>
    /// Uses Spire Doc Free library to convert .doc to .docx
    /// </summary>
    public static class WordConverter
    {
        /// <summary>
        /// Convert doc file to docx file with same name in same folder a input
        /// </summary>
        /// <param name="pathToDoc">Full path to doc file</param>
        /// <returns>Path to converted docx file</returns>
        public static string ConvertDocToDocx(string pathToDoc)
        {
            var d = new Document();
            var outpath = pathToDoc.Replace(".doc", ".docx",StringComparison.InvariantCultureIgnoreCase);
            if (File.Exists(outpath))
            {
                File.Delete(outpath);
            }

            d.LoadFromFile(pathToDoc,FileFormat.Doc);
            d.SaveToFile(outpath,FileFormat.Docx);
            return outpath;
        }

        /// <summary>
        /// Converts .doc format stream to .docx format
        /// </summary>
        /// <param name="docStream">stream of doc file</param>
        /// <returns><see cref="MemoryStream"/> of converted docx file</returns>
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
