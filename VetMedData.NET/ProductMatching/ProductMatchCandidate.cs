using System;
using System.Collections.Generic;
using System.Text;

namespace VetMedData.NET.ProductMatching
{
    public class ProductMatchCandidate
    {
        public string Name { get; set; }
        public DateTime? ActionDate { get; set; }
        public string[] TargetSpecies { get; set; }
    }
}
