using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace VetMedData.NET.Util
{
    public static class SPCParser
    {
        private const string TargetSpeciesPattern
            = @"(?<=[0-9]\s*target species\s+)([^0-9]*)";

        private const string UnbracketedAndPattern =
            @"(?<!\(\w+ +)and(?! +\w+\))";

        public static string[] GetTargetSpecies(WordprocessingDocument d)
        {
            var sb = new StringBuilder();

            foreach (var e in d.MainDocumentPart.Document.Body)
            {
                sb.Append(GetPlainText(e));
                sb.Append(Environment.NewLine);
            }

            var doctext = sb.ToString();
            return GetTargetSpeciesFromText(doctext);
        }

        public static string[] GetTargetSpeciesFromPdf(string pathToPdf)
        {
            return GetTargetSpeciesFromText(GetPlainText(pathToPdf));
        }

        public static Dictionary<string, string[]> GetTargetSpeciesFromMultiProductPdf(string pathToPdf)
        {
            var outDic = new Dictionary<string,string[]>();
            var pt = GetPlainText(pathToPdf);
            var splitPt = pt.Split(new[] {"NAME OF THE VETERINARY MEDICINAL PRODUCT"},
                StringSplitOptions.RemoveEmptyEntries);
            const string secondSectionPattern = @"2\.";
            foreach (var subdoc in splitPt.TakeLast(splitPt.Length-1))
            {
                var cleanedsubdoc = subdoc.Replace("en-GB", "").Replace("en-US","");
                var name = cleanedsubdoc.Substring(0, Regex.Matches(cleanedsubdoc, secondSectionPattern)[0].Index).Trim();
                outDic.Add(name,GetTargetSpeciesFromText(cleanedsubdoc));
            }

            return outDic;
        }

        private static string[] GetTargetSpeciesFromText(string plainText)
        {
            var spRegex = new Regex(TargetSpeciesPattern
                , RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var m = spRegex.Match(plainText);

            return Regex.Replace(m.Value.Trim().ToLowerInvariant(), UnbracketedAndPattern, ",", RegexOptions.Compiled)
                .Split(',')
                .Select(s => s.Trim().Replace(".", "")).ToArray();
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

        public static string GetPlainText(string pathToPdf)
        {
            
            var pdf = new PdfReader(pathToPdf);
            
                var sb = new StringBuilder();
                for (var i = 1; i < pdf.NumberOfPages; i++)
                {
                    var streamBytes = pdf.GetPageContent(i);
                    var tokenizer = new PrTokeniser(new RandomAccessFileOrArray(streamBytes));
                    
                    while (tokenizer.NextToken())
                    {
                        if (tokenizer.TokenType == PrTokeniser.TK_STRING)
                        {
                            sb.Append(tokenizer.StringValue);
                        }
                    }

                    sb.AppendLine();
                    if (sb.ToString().Contains("ANNEX II"))
                    {
                        break;
                    }
                }
            pdf.Close();
            return sb.ToString();
            
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
