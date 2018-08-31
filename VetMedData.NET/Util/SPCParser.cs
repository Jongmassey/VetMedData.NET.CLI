using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VetMedData.NET.Util
{
    /// <summary>
    /// Methods for parsing Summary Product Characteristics documents
    /// in either VMD (.doc/docx) formats or EMA multi-product pdf formats.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class SPCParser
    {
        //pattern for section preceding target species information
        private const string TargetSpeciesLookbehind
            = @"(?<=target species[;:\.]*\s+)";

        //pattern for section containing target species information
        private const string TargetSpeciesCaptureGroup
            = @"([\s\w\(\)\.\,\-\–\≤\≥\&]*)";
        
        //pattern for section following target species information
        private const string TargetSpeciesLookahead
            = @"(?=\s+4\.2)*(?=\s*indications* for)";

        //pattern for section following target species information (old-style document)
        private const string TargetSpeciesLookaheadOldFormat
            = @"(?=5\.2\s*therapeutic\s*indications)";

        //regex for "and" not within ()
        private const string UnbracketedAndPattern =
            @"(?<!\(\w+ +)and(?! +\w+\))";

        //regex to get Product Name section of document
        private const string ProductNamePattern = @"2\. ";

        /// <summary>
        /// Extracts target species from .docx <see cref="WordprocessingDocument"/>
        /// </summary>
        /// <param name="d">SPC document</param>
        /// <returns>string array of Target Species</returns>
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

        /// <summary>
        /// Extracts target species from .pdf
        /// </summary>
        /// <param name="pathToPdf">Path to PDF SPC document</param>
        /// <returns>string array of Target Species</returns>
        public static string[] GetTargetSpeciesFromPdf(string pathToPdf)
        {
            return GetTargetSpeciesFromText(GetPlainText(pathToPdf));
        }

        /// <summary>
        /// Extracts names of products and their target species from EMA-format multi-product PDF
        /// </summary>
        /// <param name="pathToPdf">Path to PDF SPC document</param>
        /// <returns>Dictionary of product names and their target species</returns>
        public static Dictionary<string, string[]> GetTargetSpeciesFromMultiProductPdf(string pathToPdf)
        {
            var outDic = new Dictionary<string, string[]>();
            var pt = GetPlainText(pathToPdf);

            //split plaintext into sub-SPC-document 
            var splitPt = pt.Split(new[] { "NAME OF THE VETERINARY MEDICINAL PRODUCT" },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var subdoc in splitPt.TakeLast(splitPt.Length - 1))
            {
                //plaintext output for some documents littered with locale strings that impede parsing
                var cleanedsubdoc = subdoc.Replace("en-GB", "").Replace("en-US", "");

                //sub-document might contain single line for single product, or multiple lines for multiple products
                var names = cleanedsubdoc.Substring(0, Regex.Matches(cleanedsubdoc, ProductNamePattern)[0].Index)
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(n => n.Trim())
                    .Where(n => !string.IsNullOrWhiteSpace(n));

                //in case of multiple products in same sub-document, all ahve same target species
                var ts = GetTargetSpeciesFromText(cleanedsubdoc);
                foreach (var name in names)
                {
                    outDic.Add(name, ts);
                }
            }

            return outDic;
        }

        /// <summary>
        /// Uses regex to extract target species section then cleans and reformats
        /// </summary>
        /// <param name="plainText">SPC document plaintext</param>
        /// <returns>string array of target species</returns>
        private static string[] GetTargetSpeciesFromText(string plainText)
        {
            var spRegex = new Regex(TargetSpeciesLookbehind +
                                    TargetSpeciesCaptureGroup +
                                    TargetSpeciesLookahead
                , RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var m = spRegex.Match(plainText);

            if (string.IsNullOrWhiteSpace(m.Value))
            {
                spRegex = new Regex(TargetSpeciesLookbehind +
                                    TargetSpeciesCaptureGroup +
                                    TargetSpeciesLookaheadOldFormat
                    , RegexOptions.Compiled | RegexOptions.IgnoreCase);
                m = spRegex.Match(plainText);
            }

            return Regex.Replace(m.Value.Trim().ToLowerInvariant(), UnbracketedAndPattern, ",", RegexOptions.Compiled)
                .Replace(")", "),")
                .Replace('\n', ',')
                .Replace("\r", "")
                .Split(',')
                .Select(s => s.Trim().Replace(".", ""))
                .Where(s => !string.IsNullOrWhiteSpace(s)
                && !decimal.TryParse(s, out _))
                .ToArray();
        }

        /// <summary>
        /// Extracts target species from .docx
        /// </summary>
        /// <param name="pathToSPC"> path to SPC document</param>
        /// <returns>string array of Target Species</returns>
        public static string[] GetTargetSpecies(string pathToSPC)
        {
            return GetTargetSpecies(WordprocessingDocument.Open(pathToSPC, false));
        }

        /// <summary>
        /// Extracts target species from stream of .docx
        /// </summary>
        /// <param name="s"> stream of SPC document</param>
        /// <returns>string array of Target Species</returns>
        public static string[] GetTargetSpecies(Stream s)
        {
            s.Seek(0, SeekOrigin.Begin);

            return GetTargetSpecies(WordprocessingDocument.Open(s, false));
        }

        /// <summary>
        /// Uses <see cref="iTextSharp"/> library to extract plaintext
        /// from pdf file.
        /// </summary>
        /// <param name="pathToPdf">Path to PDF file</param>
        /// <returns>Plaintext string</returns>
        public static string GetPlainText(string pathToPdf)
        {
            var pdf = new PdfReader(pathToPdf);
            //string builder for output
            var sb = new StringBuilder();

            //go page-by-page
            for (var i = 1; i < pdf.NumberOfPages; i++)
            {
                var streamBytes = pdf.GetPageContent(i);
                var tokeniser = new PrTokeniser(new RandomAccessFileOrArray(streamBytes));

                while (tokeniser.NextToken())
                {
                    switch (tokeniser.TokenType)
                    {
                        //string tokens seem to encompass everything we're interested in
                        case PrTokeniser.TK_STRING:
                            sb.Append(tokeniser.StringValue);
                            break;

                        //todo:find consistent way of parsing newlines
                        //newline tokens. Seem to be inconsistent, hacky to 
                        //add new cases as seen but works for now.
                        case PrTokeniser.TK_NUMBER:
                            if (tokeniser.StringValue.Equals("-1.159"))
                            {
                                sb.Append(Environment.NewLine);
                            }

                            break;
                        case PrTokeniser.TK_OTHER:
                            if (tokeniser.StringValue.Equals("BDC"))
                            {
                                sb.Append(Environment.NewLine);
                            }

                            break;

                        // // these are apparently the newline tokens. Results in LOTS of newlines which breaks parsing
                        //    switch (tokeniser.StringValue)
                        //    {
                        //       // case "ET":
                        //        case "TD":
                        //        case "Td":
                        //        //case "Tm":
                        //        //case "T*":
                        //            //sb.Append(Environment.NewLine);
                        //            sb.Append($"[{tokeniser.StringValue}]");
                        //            break;
                        //        default:
                        //            break;
                        //    }

                        //    break;

                        // ReSharper disable once RedundantEmptySwitchSection - keep for debugging
                        default:
                            //if (Debugger.IsAttached) { sb.Append($"[{tokeniser.TokenType}-{tokeniser.StringValue}]"); }
                            break;
                    }
                }
                // add newline between pages
                sb.AppendLine();

                //ignore latter sections
                if (sb.ToString().Contains("ANNEX II"))
                {
                    break;
                }
            }

            //close reader
            pdf.Close();
            return sb.ToString();
        }

        /// <summary> 
        ///  Read Plain Text in all <see cref="OpenXmlElement"/>s of word document
        ///  Taken from https://code.msdn.microsoft.com/office/CSOpenXmlGetPlainText-554918c3
        ///  MS-PL Licensed
        /// </summary> 
        /// <param name="element">OpenXmlElement in document</param> 
        /// <returns>Plain Text within XmlElement</returns> 
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


                    case "cr": // Carriage return 
                    case "br": // Page break 
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