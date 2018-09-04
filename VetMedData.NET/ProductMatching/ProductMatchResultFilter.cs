using System;
using System.Collections.Generic;
using System.Linq;
using VetMedData.NET.Model;

namespace VetMedData.NET.ProductMatching
{
    public interface IProductMatchResultFilter
    {
        //public Func<IEnumerable<ProductSimilarityResult>, IEnumerable<ProductSimilarityResult>> FilterFunc { get; set; }
        IEnumerable<ProductSimilarityResult> FilterResults(IEnumerable<ProductSimilarityResult> results);
    }

    public class MaximalSimilarityResultFilter : IProductMatchResultFilter
    {
        public IEnumerable<ProductSimilarityResult> FilterResults(IEnumerable<ProductSimilarityResult> results)
        {
            var productSimilarityResults = results as ProductSimilarityResult[] ?? results.ToArray();
            var maxSim = productSimilarityResults.Select(rm => rm.ProductNameSimilarity).Max();
            return productSimilarityResults.Where(r => r.ProductNameSimilarity == maxSim);
        }
    }

    public class ThresholdSimilarityResultFilter : IProductMatchResultFilter
    {
        private readonly double _thresholdValue;

        public ThresholdSimilarityResultFilter(double thresholdValue)
        {
            _thresholdValue = thresholdValue;
        }

        public IEnumerable<ProductSimilarityResult> FilterResults(IEnumerable<ProductSimilarityResult> results)
        {
            return results.Where(r => r.ProductNameSimilarity >= _thresholdValue);
        }
    }

    public class CommonTargetSpeciesFilter : IProductMatchResultFilter
    {
        public IEnumerable<ProductSimilarityResult> FilterResults(IEnumerable<ProductSimilarityResult> results)
        {
            return results.Where(r =>
                r.InputProduct.TargetSpecies != null &&
                r.ReferenceProduct.TargetSpecies != null &&
                r.InputProduct.TargetSpecies.Intersect(r.ReferenceProduct.TargetSpecies).Any()
            );
        }
    }

    public class RandomSelectFilter : IProductMatchResultFilter
    {
        public IEnumerable<ProductSimilarityResult> FilterResults(IEnumerable<ProductSimilarityResult> results)
        {
            var r = new Random();
            return new[] { results.ElementAt(r.Next(1, results.Count())) };
        }
    }

    public class CommonTimeSpanFilter : IProductMatchResultFilter
    {
        public TimeSpan DatePadding { get; set; }

        public IEnumerable<ProductSimilarityResult> FilterResults(IEnumerable<ProductSimilarityResult> results)
        {
            return results.Where(r =>
                r.InputProduct.ActionDate >= r.ReferenceProduct.GetProductActiveRange().Item1 - DatePadding &&
                r.InputProduct.ActionDate <= r.ReferenceProduct.GetProductActiveRange().Item2 + DatePadding);
        }
    }
}
