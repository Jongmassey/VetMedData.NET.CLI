using System.Collections.Generic;
using System.Linq;
using VetMedData.NET.Model;

namespace VetMedData.NET.ProductMatching
{
    public class ProductMatchRunner
    {
        private readonly ProductMatchConfig _cfg;

        public ProductMatchRunner(ProductMatchConfig cfg)
        {
            _cfg = cfg;
        }

        private IEnumerable<ProductMatchResult> GetMatchResults(Product p, IEnumerable<Product> referenceProductSet)
        {
            return referenceProductSet.Select(rp => p.GetMatchingResult(rp, _cfg));
        }

        private IEnumerable<ProductMatchResult> GetDisambiguationCandidates(IEnumerable<ProductMatchResult> MatchResults)
        {
            return _cfg.DisambiguationCandidiateFilter.FilterResults(MatchResults);
        }
    }
}
