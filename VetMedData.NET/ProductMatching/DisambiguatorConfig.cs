using System.Collections.Generic;

namespace VetMedData.NET.ProductMatching
{
    public abstract class DisambiguatorConfig
    {
    }

    public class WeightedFilterBasedDisambiguatorConfig : DisambiguatorConfig
    {
        public IDictionary<IProductMatchResultFilter, double> FiltersAndWeights { get; set; }
    }

    public class OrderedFilterBasedDisambiguatorConfig : DisambiguatorConfig
{
        public IProductMatchResultFilter[] Filters { get; set; }
    }
}
