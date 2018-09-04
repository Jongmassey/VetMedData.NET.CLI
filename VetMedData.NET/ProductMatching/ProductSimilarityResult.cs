using VetMedData.NET.Model;

namespace VetMedData.NET.ProductMatching
{
    public class ProductSimilarityResult
    {
        public ActionedProduct InputProduct { get; set; }
        public ReferenceProduct ReferenceProduct { get; set; }
        public double ProductNameSimilarity { get; set; }
    }
}
