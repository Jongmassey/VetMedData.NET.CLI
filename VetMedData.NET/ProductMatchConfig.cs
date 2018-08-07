namespace VetMedData.NET
{
    public abstract class ProductMatchConfig
    {
        public ProductNameMetricConfig NameMetricConfig { get; set; }
        public DisambiguatorConfig DisambiguatorConfig { get; set; }
        public IProductMatchResultFilter DisambiguationCandidiateFilter { get; set; }
    }

}
