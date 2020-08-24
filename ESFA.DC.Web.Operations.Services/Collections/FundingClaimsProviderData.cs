using ESFA.DC.Web.Operations.Interfaces.Collections;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class FundingClaimsProviderData : ICollection
    {
        public string CollectionName => "FundingClaimsProviderData";

        public string ReportName => "FundingClaimsProviderDataRD-ValidationReport";

        public string DisplayName => "Funding Claims Provider Data";

        public string HubName => "fundingClaimsProviderDataHub";

        public string FileFormat => ".csv";
    }
}
