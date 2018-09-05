namespace VetMedData.NET.ProductMatching
{
    public abstract class ProductMatchConfig
    {
        public ProductNameMetricConfig NameMetricConfig { get; set; }
        public IProductMatchDisambiguator Disambiguator { get; set; }
        public IProductMatchResultFilter DisambiguationCandidiateFilter { get; set; }
    }

    public class DefaultProductMatchConfig : ProductMatchConfig
    {
        public DefaultProductMatchConfig()
        {
            NameMetricConfig = new DefaultProductNameMetricConfig();
            Disambiguator = new HierarchicalFilterWithRandomFinalSelect(
                new OrderedFilterBasedDisambiguatorConfig
                {
                    Filters = new IProductMatchResultFilter[]
                    {
                        new CommonTargetSpeciesFilter() ,
                        new RandomSelectFilter()
                    }
                });
            DisambiguationCandidiateFilter = new MaximalSimilarityResultFilter();
        }
    }

}
