using System;
using System.Collections.Generic;
using System.Text;

namespace VetMedData.NET
{
    class ProductMatchingConfig
    {
        public ProductNameMetricConfig NameMetricConfig { get; set; }
        public DisambiguatorConfig DisambiguatorConfig { get; set; }
    }
}
