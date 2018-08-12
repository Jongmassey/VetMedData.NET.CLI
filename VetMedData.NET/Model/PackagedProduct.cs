using System;

namespace VetMedData.NET.Model
{
    public class AdministeredProduct : PackagedProduct
    {
        public DateTime AdministeredDateTime { get; set; }
        public string AdministeredBy { get; set; }
        public string ReasonForAdministration { get; set; }
        public double ProportionOfPackageAdministered { get; set; }
    }

    public class SoldProduct : PackagedProduct
    {
        public DateTime SoldDateTime { get; set; }
        public string SoldBy { get; set; }
    }

    public abstract class PackagedProduct
    {
        public Product Product { get; set; }
        public DateTime Expiry { get; set; }
        public string Batch { get; set; }
        public Package Package { get; set; }
    }
}
