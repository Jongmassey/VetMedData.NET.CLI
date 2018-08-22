using System.Collections.Generic;

namespace VetMedData.NET.Model
{
    public class Package
    {
        public PackageType PackageType { get; set; }
        public float PackageSize { get; set; }
        public string DisplayName => $"{PackageSize} {PackageType}";

        public override string ToString()
        {
            return DisplayName;
        }

        public static IList<Package> DefaultPackages => new List<Package>
        {
            new Package {PackageType = new PackageType{Name ="Tube",Unit = "item"}},
            new Package {PackageType = new PackageType{Name ="Bottle", Unit ="ml"}},
            new Package {PackageType = new PackageType{Name ="Spray" , Unit = "ml"}},
            new Package {PackageType = new PackageType{Name ="Bolus", Unit = "ml"}}
        };
    }

    public class PackageType
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public class PackageTypeEqualityComparer : IEqualityComparer<PackageType>
        {
            public bool Equals(PackageType x, PackageType y)
            {
                return x.Unit.Equals(y.Unit) && x.Name.Equals(y.Name);
            }

            public int GetHashCode(PackageType obj)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
