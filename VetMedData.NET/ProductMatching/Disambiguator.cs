using System.Collections.Generic;
using System.Linq;

namespace VetMedData.NET.ProductMatching
{
    public interface IProductMatchDisambiguator
    {
        ProductSimilarityResult DisambiguateMatchResults(IEnumerable<ProductSimilarityResult> results);
    }

    public class HierarchicalFilterWithRandomFinalSelect : IProductMatchDisambiguator
    {
        private readonly OrderedFilterBasedDisambiguatorConfig _cfg;

        public HierarchicalFilterWithRandomFinalSelect(OrderedFilterBasedDisambiguatorConfig cfg)
        {
            _cfg = cfg;
        }

        public ProductSimilarityResult DisambiguateMatchResults(IEnumerable<ProductSimilarityResult> results)
        {
            results = _cfg.Filters.Aggregate(results,
                (current, filter) =>
                {
                    var productSimilarityResults = current as ProductSimilarityResult[] ?? current.ToArray();
                    return (productSimilarityResults.Count() > 1
                        ? filter.FilterResults(productSimilarityResults).Any()
                            ?
                            filter.FilterResults(productSimilarityResults)
                            : productSimilarityResults
                        : productSimilarityResults);
                });

            var similarityResults = results as ProductSimilarityResult[] ?? results.ToArray();
            return similarityResults.Length > 1 ?
                new RandomSelectFilter().FilterResults(similarityResults).Single() :
                similarityResults.Single();
        }
    }

    public class ParallelWeightedFilterWithMaxThenRandomFinalSelect : IProductMatchDisambiguator
    {
        private readonly WeightedFilterBasedDisambiguatorConfig _cfg;

        public ParallelWeightedFilterWithMaxThenRandomFinalSelect(WeightedFilterBasedDisambiguatorConfig cfg)
        {
            _cfg = cfg;
        }

        public ProductSimilarityResult DisambiguateMatchResults(IEnumerable<ProductSimilarityResult> results)
        {
            var resultDisambiguationScores = results.ToDictionary(r => r, r => 0d);
            foreach (var filterAndWeight in _cfg.FiltersAndWeights)
            {
                var weight = filterAndWeight.Value;
                foreach (var pmr in filterAndWeight.Key.FilterResults(results))
                {
                    resultDisambiguationScores[pmr] += weight;
                }
            }

            var maxdmbscore = resultDisambiguationScores.Values.Max();
            results = resultDisambiguationScores
                .Where(rdb => rdb.Value == maxdmbscore)
                .Select(r => r.Key);

            return results.Count() > 1 ?
                new RandomSelectFilter().FilterResults(results).Single() :
                results.Single();
        }
    }

}
