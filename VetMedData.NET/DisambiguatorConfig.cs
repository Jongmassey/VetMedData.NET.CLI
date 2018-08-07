using System.Collections.Generic;
using System.Collections.Specialized;

namespace VetMedData.NET
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
