using System;
using System.Collections.Generic;

namespace VetMedData.NET
{
    /// <summary>
    /// Defines structure of Product Information Database
    /// from Veterinary Medicines Directorate
    /// </summary>
    
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

    /// <summary>
    /// Defines properties common to all product types
    /// </summary>
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
    /// <summary>
    /// Product with a current, active marketing authorisation
    /// </summary>
    public class CurrentlyAuthorisedProduct : Product
    {
        public IEnumerable<string> Distributors { get; set; }
    }
    /// <summary>
    /// Product whose marketing authorisation has been suspended
    /// </summary>
    public class SuspendedProduct : Product
    {
        public DateTime DateOfSuspension { get; set; }
    }
    /// <summary>
    /// Product whose marketing authorisation has expired
    /// </summary>
    public class ExpiredProduct : Product
    {
        public DateTime DateofExpiration { get; set; }
    }
    /// <summary>
    /// Authorised homoepathic product
    /// </summary>
    public class HomoeopathicProduct : Product
    {
    }

   
}