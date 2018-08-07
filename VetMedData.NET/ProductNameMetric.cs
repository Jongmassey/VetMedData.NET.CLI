using System;
using SimMetrics.Net.API;

namespace VetMedData.NET
{
    public class ProductNameMetric :AbstractStringMetric
    {
        private ProductNameMetricConfig _config;
        public ProductNameMetric(ProductNameMetricConfig conf)
        {
            _config = conf;
        }

        /// <summary>
        /// Do Not Use. Use ProductNameMetric(ProductNameMetricConfig conf) instead.
        /// </summary>
        public ProductNameMetric()
        {
            throw new NotImplementedException("Configuration required at construction");
        }

        public override double GetSimilarity(string firstWord, string secondWord)
        {
            throw new NotImplementedException();
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
