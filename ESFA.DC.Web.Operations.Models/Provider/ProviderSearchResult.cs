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

        public ProviderSearchResult(string providerName, long ukprn, string upin, string tradingName)
        {
            ProviderName = providerName;
            Ukprn = ukprn;
            Upin = upin;
            TradingName = tradingName;
        }

        public string ProviderName { get; set; }
        
        public long Ukprn { get; set; }
     
        public string Upin { get; set; }

        public string TradingName { get; set; }
    }
}
