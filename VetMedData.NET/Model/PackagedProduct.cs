using System;

namespace VetMedData.NET.Model
{
    /// <inheritdoc />
    /// <summary>
    /// Record of administration of portion of a packaged product
    /// </summary>
    public class AdministeredProduct : PackagedProduct
    {
        public DateTime AdministeredDateTime { get; set; }
        public string AdministeredBy { get; set; }
        public string ReasonForAdministration { get; set; }
        public double ProportionOfPackageAdministered { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// Record of sale of packaged product
    /// </summary>
    public class SoldProduct : PackagedProduct
    {
        public DateTime SoldDateTime { get; set; }
        public string SoldBy { get; set; }
    }

    /// <summary>
    /// Properties of package of a product
    /// </summary>
    public abstract class PackagedProduct
    {
        public Product Product { get; set; }
        public DateTime Expiry { get; set; }
        public string Batch { get; set; }
        public Package Package { get; set; }
    }
}
