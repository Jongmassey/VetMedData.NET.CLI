using SimMetrics.Net.API;
using SimMetrics.Net.Metric;
using SimMetrics.Net.Utilities;

namespace VetMedData.NET
{
    /// <summary>
    /// Configures medicine similarity metric behaviour
    /// </summary>
    public abstract class ProductNameMetricConfig
    {
        /// <summary>
        /// String similarity metric to use when comparing names
        /// </summary>
        public AbstractStringMetric InnerMetric { get; set; }
        /// <summary>
        /// Name pre-cleaning configuration
        /// </summary>
        public NameCleaningConfig NameCleaningConfig { get; set; }
        /// <summary>
        /// Tokeniser for pairwise token similarity measurement.
        /// If left null then whole name strings will be compared.
        /// </summary>
        public ITokeniser Tokeniser { get; set; }
        /// <summary>
        /// Power to which the token index will be raised to form
        /// inverse weighting coefficient for A string
        /// </summary>
        public float APositionalWeightingCoefficientPower { get; set; }
        /// <summary>
        /// Power to which the token index will be raised to form
        /// inverse weighting coefficient for B string
        /// </summary>
        public float BPositionalWeightingCoefficientPower { get; set; }
        /// <summary>
        /// Ratio of A-token-index-weighting (0) to
        /// B-token-index-weighting (1) to be applied.
        /// </summary>
        public float ABCompoundPositionalWeightRatio { get; set; }
    }

    public class DefaultProductNameMetricConfig : ProductNameMetricConfig
    {
        public DefaultProductNameMetricConfig()
        {
            InnerMetric = new Levenstein();
            NameCleaningConfig = new DefaultMatchNameCleaningConfig();
            Tokeniser = new TokeniserWhitespace();
            APositionalWeightingCoefficientPower = 0f;
            BPositionalWeightingCoefficientPower = 1.5f;
            ABCompoundPositionalWeightRatio = 1f;
        }
    }
}
