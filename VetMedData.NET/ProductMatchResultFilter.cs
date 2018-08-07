using System;
using System.Collections.Generic;
using System.Linq;

namespace VetMedData.NET
{
    public interface IProductMatchResultFilter
    {
        //public Func<IEnumerable<ProductMatchResult>, IEnumerable<ProductMatchResult>> FilterFunc { get; set; }
        IEnumerable<ProductMatchResult> FilterResults(IEnumerable<ProductMatchResult> results);
    }

    public class MaximalSimilarityResultFilter : IProductMatchResultFilter
    {
        public IEnumerable<ProductMatchResult> FilterResults(IEnumerable<ProductMatchResult> results)
        {
            return results.Where(r => r.ProductNameSimilarity == results.Select(rm => rm.ProductNameSimilarity).Max());
        }
    }

    public class ThresholdSimilarityResultFilter : IProductMatchResultFilter
    {
        private readonly double _thresholdValue;

        public ThresholdSimilarityResultFilter(double thresholdValue)
        {
            _thresholdValue = thresholdValue;
        }

        public IEnumerable<ProductMatchResult> FilterResults(IEnumerable<ProductMatchResult> results)
        {
            return results.Where(r => r.ProductNameSimilarity >= _thresholdValue);
        }
    }

    public class CommonTargetSpeciesFilter : IProductMatchResultFilter
    {
        public IEnumerable<ProductMatchResult> FilterResults(IEnumerable<ProductMatchResult> results)
        {
            return results.Where(r =>
                r.InputProduct.GetType().GetProperties().Any(p => p.Name.Equals("TargetSpecies"))
                &&
                r.ReferenceProduct.GetType().GetProperties().Any(p => p.Name.Equals("TargetSpecies"))
                && ((IEnumerable<string>)r.InputProduct.GetType()
                    .GetProperties()
                    .Single(p => p.Name.Equals("TargetSpecies")).GetValue(r.InputProduct))
                  .Intersect(
                    (IEnumerable<string>)r.ReferenceProduct.GetType()
                      .GetProperties()
                      .Single(p => p.Name.Equals("TargetSpecies")).GetValue(r.ReferenceProduct))
                  .Any()
            );
        }
    }

    public class RandomSelectFilter : IProductMatchResultFilter
    {
        public IEnumerable<ProductMatchResult> FilterResults(IEnumerable<ProductMatchResult> results)
        {
            var r = new Random();
            return new[] { results.ElementAt(r.Next(1, results.Count())) };
        }


    }
}
