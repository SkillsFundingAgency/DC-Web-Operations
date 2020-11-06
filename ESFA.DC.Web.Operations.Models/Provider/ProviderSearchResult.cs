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

        public ProviderSearchResult(string providerName, long ukprn, string upin, string tradingName, bool existsInSld)
        {
            ProviderName = providerName;
            Ukprn = ukprn;
            Upin = upin;
            TradingName = tradingName;
            ExistsInSld = existsInSld;
        }

        public string ProviderName { get; set; }

        public long Ukprn { get; set; }

        public string Upin { get; set; }

        public string TradingName { get; set; }

        public bool ExistsInSld { get; set; }
    }
}
