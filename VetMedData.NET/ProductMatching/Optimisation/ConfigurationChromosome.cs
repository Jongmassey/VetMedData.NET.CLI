using GeneticSharp.Domain.Chromosomes;

namespace VetMedData.NET.ProductMatching.Optimisation
{
    internal class ConfigurationChromosome:FloatingPointChromosome
    {
        public ConfigurationChromosome(double[] minValue, double[] maxValue, int[] totalBits, int[] fractionDigits) : base(minValue, maxValue, totalBits, fractionDigits)
        {
        }

        public ProductMatchConfig GetMatchConfig()
        {
            return new DefaultProductMatchConfig
            {
                NameMetricConfig =
                {
                    ABCompoundPositionalWeightRatio = 0d,
                    APositionalWeightingCoefficientPower = 0d,
                    BPositionalWeightingCoefficientPower = 0d
                }
            };
        }

        //public DisambiguatorConfig GetDisambiguatorConfig(ConfigurationChromosome chromosome)
        //{
        //    var threshold = 0d;
        //    return new OrderedFilterBasedDisambiguatorConfig
        //    {
        //        Filters = new IProductMatchResultFilter[]
        //        {
        //            new CommonTargetSpeciesFilter(),
        //            new ThresholdSimilarityResultFilter(threshold)
        //        }
        //    };
        //}
    }
}
