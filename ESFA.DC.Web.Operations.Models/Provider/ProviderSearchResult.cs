namespace ESFA.DC.Web.Operations.Models.Provider
{
    public class ProviderSearchResult
    {
        public ProviderSearchResult(string providerName, long ukprn, string upin)
        {
            ProviderName = providerName;
            Ukprn = ukprn;
            Upin = upin;
        }

        public string ProviderName { get; set; }
        
        public long Ukprn { get; set; }
     
        public string Upin { get; set; }
    }
}
