namespace ESFA.DC.Web.Operations.Models.Provider
{
    public class ProviderSearchResult
    {
        public ProviderSearchResult(string providerName, long ukprn, int upin)
        {
            ProviderName = providerName;
            Ukprn = ukprn;
            Upin = upin;
        }

        public string ProviderName { get; set; }
        
        public long Ukprn { get; set; }
     
        public int Upin { get; set; }
    }
}
