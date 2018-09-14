using GeneticSharp.Domain.Chromosomes;

namespace VetMedData.NET.ProductMatching.Optimisation
{
    internal class ConfigurationChromosome : FloatingPointChromosome
    {
        public ConfigurationChromosome() : base(
            new[] { 0d, 0d, 0d, 0d }
            , new[] { 1d, 10d, 10d, 1d }
            , new[] { 10, 10, 10, 10 }
            , new[] { 3, 3, 3, 3 }){}
    }

    internal static class ChromosomeExtensions
    {
        public static ProductMatchConfig GetMatchConfig(this FloatingPointChromosome fpc)
        {
            var values = fpc.ToFloatingPoints();
            return new DefaultProductMatchConfig
            {
                NameMetricConfig =
                {
                    ABCompoundPositionalWeightRatio = values[0],
                    APositionalWeightingCoefficientPower = values[1],
                    BPositionalWeightingCoefficientPower = values[2]
                }
            };
        }
    }
}
