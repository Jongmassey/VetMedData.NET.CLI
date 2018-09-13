using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VetMedData.NET.Model;
using VetMedData.NET.Util;

namespace VetMedData.NET.ProductMatching.Optimisation
{
    internal class CorrectPercentageFitness : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            var correctCount = 0;
            var pid = VMDPIDFactory.GetVmdPid().Result;
            
            var cfg = (ConfigurationChromosome)chromosome;
            var pmr = new ProductMatchRunner(cfg.GetMatchConfig());
            var toMatch = new ConcurrentDictionary<string, string[]>(TruthFactory.GetTruth());
            Parallel.ForEach(toMatch, tm =>
            {
                var ap = new ActionedProduct { Product = new Product { Name = tm.Key }, TargetSpecies = new[] { "cattle" } };
                ReferenceProduct[] rp;
                lock (pid)
                {
                    rp = pid.RealProducts.ToArray();
                }
                var res = pmr.GetMatch(ap, rp);
                if (tm.Value.Contains(res.ReferenceProduct.VMNo))
                {
                    Interlocked.Increment(ref correctCount);
                }
            });

            return (double)correctCount / toMatch.Count;
        }
    }
}
