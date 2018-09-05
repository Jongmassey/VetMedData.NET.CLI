using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using VetMedData.NET.Model;
using VetMedData.NET.ProductMatching;
using VetMedData.NET.Util;

namespace VetMedData.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length > 0 && File.Exists(args[0]))
            {
                var sb = new StringBuilder("\"Input Name\",\"Matched Name\",\"VM Number\",\"Similarity Score\""+Environment.NewLine);
                var pid = PersistentPID.Get(true, true);
                var cfg = new DefaultProductMatchConfig();
                var pmr = new ProductMatchRunner(cfg);
                var i = 0;
                var sw = Stopwatch.StartNew();

                var inputStrings = new BlockingCollection<string>();

                using (var fs = File.OpenText(args[0]))
                {
                    while (!fs.EndOfStream)
                    {
                        inputStrings.Add(fs.ReadLine().ToLowerInvariant().Trim());
                        i++;
                        //var ln = fs.ReadLine();
                        //var ap = new SoldProduct
                        //{
                        //    TargetSpecies = new[] { "cattle" },
                        //    Product = new Product { Name = ln.ToLowerInvariant().Trim() },
                        //    ActionDate = DateTime.Now
                        //};
                        //var res = pmr.GetMatch(ap, pid.RealProducts);
                        //sb.AppendJoin(',', $"\"{res.InputProduct.Product.Name}\"", $"\"{res.ReferenceProduct.Name}\"",
                        //    res.ProductNameSimilarity.ToString(CultureInfo.InvariantCulture));
                        //sb.AppendLine();
                    }
                }
                Console.WriteLine($"Read {i} rows in {string.Format("{0:0.00}", sw.Elapsed.TotalSeconds)} seconds.");
                sw.Restart();
                Parallel.ForEach(inputStrings, inputString =>
                {
                    var ap = new SoldProduct
                    {
                        TargetSpecies = new[] { "cattle" },
                        Product = new Product { Name = inputString },
                        ActionDate = DateTime.Now
                    };

                    var res = pmr.GetMatch(ap, pid.RealProducts);
                    lock (sb)
                    {
                        sb.AppendJoin(',',
                            $"\"{res.InputProduct.Product.Name}\"",
                            $"\"{res.ReferenceProduct.Name}\"",
                            $"\"{res.ReferenceProduct.VMNo}\"",
                            res.ProductNameSimilarity.ToString(CultureInfo.InvariantCulture),
                            Environment.NewLine);
                    }
                });
                Console.WriteLine($"Processed {i} rows in {string.Format("{0:0.00}", sw.Elapsed.TotalSeconds)} seconds.");
                sw.Restart();
                var outfile = args.Length==2? Path.GetFileName(args[0])+ args[1]: args[0] + ".out.csv";
                File.WriteAllText(outfile, sb.ToString());
                Console.WriteLine($"Wrote {i} rows in {string.Format("{0:0.00}", sw.Elapsed.TotalSeconds)} seconds.");
            }
            else
            {
                Console.WriteLine("Requires path to file to process as first argument.");
                Console.WriteLine("Output will be generated in same location unless path specified in second argument.");
                Console.ReadLine();
            }
        }
    }
}
