using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;


namespace VetMedData.NET
{
    public static class SPCParser
    {
        private const string TargetSpeciesPattern
            = @"(?<=[0-9]\s*target species\s+)([^0-9]*)";

        public static string[] GetTargetSpecies(WordprocessingDocument d)
        {
            var sb = new StringBuilder();

            foreach (var e in d.MainDocumentPart.Document.Body)
            {
                sb.Append(GetPlainText(e));
                sb.Append(Environment.NewLine);
            }

            var doctext = sb.ToString();
            var spRegex = new Regex(TargetSpeciesPattern
                , RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var m = spRegex.Match(doctext);

            return m.Value.Trim().Split(',').Select(s => s.Trim()).ToArray();
        }

        public static string[] GetTargetSpecies(string pathToSPC)
        {
            return GetTargetSpecies(WordprocessingDocument.Open(pathToSPC, false));
        }

        public static string[] GetTargetSpecies(Stream s)
        {
            s.Seek(0, SeekOrigin.Begin);

            return GetTargetSpecies(WordprocessingDocument.Open(s, false));
        }

        /// <summary> 
        ///  Read Plain Text in all XmlElements of word document
        ///  Taken from https://code.msdn.microsoft.com/office/CSOpenXmlGetPlainText-554918c3
        ///  MS-PL Licensed
        /// </summary> 
        /// <param name="element">XmlElement in document</param> 
        /// <returns>Plain Text in XmlElement</returns> 
        public static string GetPlainText(OpenXmlElement element)
        {
            StringBuilder PlainTextInWord = new StringBuilder();
            foreach (OpenXmlElement section in element.Elements())
            {
                switch (section.LocalName)
                {
                    // Text 
                    case "t":
                        PlainTextInWord.Append(section.InnerText);
                        break;


                    case "cr":                          // Carriage return 
                    case "br":                          // Page break 
                        PlainTextInWord.Append(Environment.NewLine);
                        break;


                    // Tab 
                    case "tab":
                        PlainTextInWord.Append("\t");
                        break;


                    // Paragraph 
                    case "p":
                        PlainTextInWord.Append(GetPlainText(section));
                        PlainTextInWord.AppendLine(Environment.NewLine);
                        break;


                    default:
                        PlainTextInWord.Append(GetPlainText(section));
                        break;
                }
            }


            return PlainTextInWord.ToString();
        }
    }
}
