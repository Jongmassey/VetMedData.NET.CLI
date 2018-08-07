using System;
using SimMetrics.Net.API;

namespace VetMedData.NET
{
    public class ProductNameMetric : AbstractStringMetric
    {
        private readonly ProductNameMetricConfig _config;
        public ProductNameMetric(ProductNameMetricConfig conf = null)
        {
            _config = conf ?? new DefaultProductNameMetricConfig();
        }
        /// <summary>
        /// Calculate average token-pair similarity weighted by token position as
        /// per ProductNameMetricConfig.
        /// </summary>
        /// <param name="firstWord">"A" input product name.</param>
        /// <param name="secondWord">"B" reference product name.</param>
        /// <returns>Product name similarity</returns>
        public override double GetSimilarity(string firstWord, string secondWord)
        {
            var aTokens = _config.Tokeniser.Tokenize(firstWord);
            var bTokens = _config.Tokeniser.Tokenize(secondWord);

            var totalSim = 0d;
            var totalDivisor = 0d;

            for (var aIndex = 0; aIndex < aTokens.Count; aIndex++)
            {
                var maxSim = 0d;
                var maxSimBIndex = 0;
                var maxSimAIndex = 0;

                for (var bIndex = 0; bIndex < bTokens.Count; bIndex++)
                {
                    var sim = _config.InnerMetric.GetSimilarity(aTokens[aIndex], bTokens[bIndex]);
                    if (sim > maxSim)
                    {
                        maxSim = sim;
                        maxSimAIndex = aIndex+1;
                        maxSimBIndex = bIndex+1;
                    }
                }

                var aWeight = Math.Pow(maxSimAIndex,_config.APositionalWeightingCoefficientPower);
                var bWeight = Math.Pow(maxSimBIndex, _config.BPositionalWeightingCoefficientPower);
                var netWeight = aWeight * (1 - _config.ABCompoundPositionalWeightRatio) +
                                bWeight * _config.ABCompoundPositionalWeightRatio;

                totalSim += maxSim / netWeight;

                totalDivisor += 1 / netWeight;
            }

            return totalSim / totalDivisor;
        }

        public override string GetSimilarityExplained(string firstWord, string secondWord)
        {
            throw new NotImplementedException();
        }

        public override double GetSimilarityTimingEstimated(string firstWord, string secondWord)
        {
            throw new NotImplementedException();
        }

        public override double GetUnnormalisedSimilarity(string firstWord, string secondWord)
        {
            throw new NotImplementedException();
        }

        public override string LongDescriptionString { get; }
        public override string ShortDescriptionString { get; }
    }
}
