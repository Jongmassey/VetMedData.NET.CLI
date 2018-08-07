using System.Collections.Generic;
using System.Linq;

namespace VetMedData.NET
{
    public interface IProductMatchDisambiguator
    {
        ProductMatchResult DisambiguateMatchResults(IEnumerable<ProductMatchResult> results);
    }

    public class HierarchicalFilterWithRandomFinalSelect : IProductMatchDisambiguator
    {
        private readonly OrderedFilterBasedDisambiguatorConfig _cfg;

        public HierarchicalFilterWithRandomFinalSelect(OrderedFilterBasedDisambiguatorConfig cfg)
        {
            _cfg = cfg;
        }

        public ProductMatchResult DisambiguateMatchResults(IEnumerable<ProductMatchResult> results)
        {
            foreach (var filter in _cfg.Filters)
            {
                results =
                    results.Count() > 1 ?
                        filter.FilterResults(results).Any() ?
                            filter.FilterResults(results) :
                            results :
                        results;
            }

            return results.Count() > 1 ?
                new RandomSelectFilter().FilterResults(results).Single() :
                results.Single();
        }
    }

    public class ParallelWeightedFilterWithMaxThenRandomFinalSelect : IProductMatchDisambiguator
    {
        private readonly WeightedFilterBasedDisambiguatorConfig _cfg;

        public ParallelWeightedFilterWithMaxThenRandomFinalSelect(WeightedFilterBasedDisambiguatorConfig cfg)
        {
            _cfg = cfg;
        }

        public ProductMatchResult DisambiguateMatchResults(IEnumerable<ProductMatchResult> results)
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
