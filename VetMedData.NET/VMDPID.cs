using System;
using System.Collections.Generic;

namespace VetMedData.NET
{
    public class VMDPID
    {
        public DateTime CreatedDateTime { get; set; }
        public List<CurrentlyAuthorisedProduct> CurrentlyAuthorisedProducts { get; set; } =
            new List<CurrentlyAuthorisedProduct>();
        public List<SuspendedProduct> SuspendedProducts { get; set; } =
            new List<SuspendedProduct>();
        public List<ExpiredProduct> ExpiredProducts { get; set; } =
            new List<ExpiredProduct>();
        public List<HomoeopathicProduct> HomoeopathicProducts { get; set; } =
            new List<HomoeopathicProduct>();

    }


    public class CurrentlyAuthorisedProduct : Product
    {
        public IEnumerable<string> Distributors { get; set; }
    }

    public class SuspendedProduct : Product
    {
        public DateTime DateOfSuspension { get; set; }
    }

    public class ExpiredProduct : Product
    {
        public DateTime DateofExpiration { get; set; }
    }

    public class HomoeopathicProduct : Product
    {
    }

    public abstract class Product
    {
        public string Name { get; set; }
        public string MAHolder { get; set; }
        public string VMNo { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string AuthorisationRoute { get; set; }
        public IEnumerable<string> ActiveSubstances { get; set; }
        public string ControlledDrug { get; set; }
        public IEnumerable<string> TargetSpecies { get; set; }
        public string DistributionCategory { get; set; }
        public string PharmaceuticalForm { get; set; }
        public string TherapeuticGroup { get; set; }
        public string SPC_Link { get; set; }
        public string UKPAR_Link { get; set; }
        public string PAAR_Link { get; set; }
    }
}