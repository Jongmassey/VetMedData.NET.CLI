using System;
using System.Collections.Generic;
using System.Linq;

namespace VetMedData.NET
{
    /// <summary>
    /// Defines structure of Product Information Database
    /// from Veterinary Medicines Directorate and provides
    /// data access methods.
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

        /// <summary>
        /// Unions together all product types 
        /// </summary>
        public IEnumerable<Product> AllProducts => CurrentlyAuthorisedProducts
            .Union(SuspendedProducts.Select(s => (Product)s))
            .Union(ExpiredProducts.Select(s => (Product)s))
            .Union(HomoeopathicProducts.Select(s => (Product)s));

        /// <summary>
        /// Unique list of all Pharmaceutical Forms across all products
        /// </summary>
        public IEnumerable<string> PharmaceuticalForms => CurrentlyAuthorisedProducts.Select(s => s.PharmaceuticalForm)
            .Union(SuspendedProducts.Select(s => s.PharmaceuticalForm))
            .Union(HomoeopathicProducts.Select(s => s.PharmaceuticalForm))
            .Distinct();

        /// <summary>
        /// Unique list of all MA Holders across all products
        /// </summary>
        public IEnumerable<string> MAHolders => CurrentlyAuthorisedProducts.Select(s => s.MAHolder)
            .Union(SuspendedProducts.Select(s => s.MAHolder))
            .Union(ExpiredProducts.Select(s => s.MAHolder))
            .Union(HomoeopathicProducts.Select(s => s.MAHolder))
            .Distinct();

        /// <summary>
        /// Unique list of all Authorisation Routes across all products
        /// </summary>
        public IEnumerable<string> AuthorisationRoutes => CurrentlyAuthorisedProducts.Select(s => s.AuthorisationRoute)
            .Union(SuspendedProducts.Select(s => s.AuthorisationRoute))
            .Union(ExpiredProducts.Select(s => s.AuthorisationRoute))
            .Union(HomoeopathicProducts.Select(s => s.AuthorisationRoute))
            .Distinct();


        /// <summary>
        /// Unique list of all Therapeutic Groups across all products
        /// </summary>
        public IEnumerable<string> TherapeuticGroups => CurrentlyAuthorisedProducts.Select(s => s.TherapeuticGroup)
            .Union(SuspendedProducts.Select(s => s.TherapeuticGroup))
            .Union(HomoeopathicProducts.Select(s => s.TherapeuticGroup))
            .Distinct();

        /// <summary>
        /// Unique list of all Active Substances across all products
        /// </summary>
        public IEnumerable<string> ActiveSubstances => CurrentlyAuthorisedProducts.SelectMany(s => s.ActiveSubstances)
            .Union(SuspendedProducts.SelectMany(s => s.ActiveSubstances))
            .Union(ExpiredProducts.SelectMany(s => s.ActiveSubstances))
            .Union(HomoeopathicProducts.SelectMany(s => s.ActiveSubstances))
            .Distinct();

        /// <summary>
        /// Unique list of all Target Species across all products
        /// </summary>
        public IEnumerable<string> TargetSpecies => CurrentlyAuthorisedProducts.SelectMany(s => s.TargetSpecies)
            .Union(SuspendedProducts.SelectMany(s => s.TargetSpecies))
            .Union(HomoeopathicProducts.SelectMany(s => s.TargetSpecies))
            .Distinct();


    }

    /// <summary>
    /// Defines properties common to all product types
    /// </summary>
    public abstract class Product
    {
        public string Name { get; set; }
        public string MAHolder { get; set; }
        public string VMNo { get; set; }
        public string AuthorisationRoute { get; set; }
        public IEnumerable<string> TargetSpecies { get; set; }
        public IEnumerable<string> ActiveSubstances { get; set; }

        public override string ToString()
        {
            return $"{GetType()}: Name:{Name} VMNo:{VMNo}";
        }
    }
    /// <summary>
    /// Product with a current, active marketing authorisation
    /// </summary>
    public class CurrentlyAuthorisedProduct : Product
    {
        public IEnumerable<string> Distributors { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string ControlledDrug { get; set; }
        public string DistributionCategory { get; set; }
        public string PharmaceuticalForm { get; set; }
        public string TherapeuticGroup { get; set; }
        public string SPC_Link { get; set; }
        public string UKPAR_Link { get; set; }
        public string PAAR_Link { get; set; }
    }
    /// <summary>
    /// Product whose marketing authorisation has been suspended
    /// </summary>
    public class SuspendedProduct : Product
    {
        public DateTime DateOfSuspension { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string ControlledDrug { get; set; }
        public string DistributionCategory { get; set; }
        public string PharmaceuticalForm { get; set; }
        public string TherapeuticGroup { get; set; }
        public string SPC_Link { get; set; }
        public string UKPAR_Link { get; set; }
        public string PAAR_Link { get; set; }
    }
    /// <summary>
    /// Product whose marketing authorisation has expired
    /// </summary>
    public class ExpiredProduct : Product
    {
        public DateTime DateofExpiration { get; set; }
        public string SPC_Link { get; set; }
    }
    /// <summary>
    /// Authorised homoepathic product
    /// </summary>
    public class HomoeopathicProduct : Product
    {
        public DateTime DateOfIssue { get; set; }
        public string ControlledDrug { get; set; }
        public string DistributionCategory { get; set; }
        public string PharmaceuticalForm { get; set; }
        public string TherapeuticGroup { get; set; }
    }


}