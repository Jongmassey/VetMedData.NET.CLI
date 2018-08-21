namespace VetMedData.NET.ProductMatching
{
    /// <summary>
    /// Default set of rules to agressively clean medicine names for
    /// name matching.
    /// </summary>
    public class DefaultMatchNameCleaningConfig : NameCleaningConfig
    {
        public DefaultMatchNameCleaningConfig()
        {
            Lowercase = true;
            RemoveBracketedTerms = true;
            RemoveLeadingMultiplierTerms = true;
            CharsToRemove = new[] {',', '.', '*'};
            TermsToRemove = new[] {"dispense"};
        }
    }
    /// <summary>
    /// Set of rules for cleaning veterinary medicine names
    /// </summary>
    public abstract class NameCleaningConfig
    {
        /// <summary>
        /// Set name to lowercase
        /// </summary>
        public bool Lowercase { get; set; }
        /// <summary>
        /// Remove terms contained within [] () including
        /// leading/trailing spaces
        /// </summary>
        public bool RemoveBracketedTerms { get; set; }
        /// <summary>
        /// Single characters to be removed at any point in
        /// name.
        /// </summary>
        public char[] CharsToRemove { get; set; }
        /// <summary>
        /// Terms to be removed so long as they are bounded
        /// by a word-boundary (space, punctuation, start/end of string)
        /// </summary>
        public string[] TermsToRemove { get; set; }
        /// <summary>
        /// Regular expression patterns for removal
        /// </summary>
        public string[] PatternsToRemove { get; set; }
        /// <summary>
        /// Remove numeric leading terms when followed by "x" or "X" along with
        /// the x.
        /// </summary>
        public bool RemoveLeadingMultiplierTerms { get; set; }
    }
}
