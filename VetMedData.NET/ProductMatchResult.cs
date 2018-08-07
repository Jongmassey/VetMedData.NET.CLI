namespace VetMedData.NET
{
    public class ProductMatchResult
    {
        public Product InputProduct { get; set; }
        public Product ReferenceProduct { get; set; }
        public double ProductNameSimilarity { get; set; }
    }
}
