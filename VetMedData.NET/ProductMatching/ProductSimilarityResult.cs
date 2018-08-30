using VetMedData.NET.Model;

namespace VetMedData.NET.ProductMatching
{
    public class ProductSimilarityResult
    {
        public ProductMatchCandidate InputProduct { get; set; }
        public Product ReferenceProduct { get; set; }
        public double ProductNameSimilarity { get; set; }
    }
}
