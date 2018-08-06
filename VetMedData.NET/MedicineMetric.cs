using System;
using System.Collections.Generic;
using System.Text;
using SimMetrics.Net.API;

namespace VetMedData.NET
{
    class MedicineMetric :AbstractStringMetric
    {
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
