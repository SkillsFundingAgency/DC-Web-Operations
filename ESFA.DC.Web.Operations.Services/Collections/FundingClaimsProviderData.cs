using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class FundingClaimsProviderData : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.ReferenceDataFundingClaimsProviderData;

        public string ReportName => ReportTypes.FundingClaimsProviderDataReportName;

        public string DisplayName => "Funding Claims Provider Data";

        public string HubName => "fundingClaimsProviderDataHub";

        public string FileFormat => FileNameExtensionConsts.CSV;

        public string FileNameFormat => "FundingClaimsProviderDataRD-YYYYMMDDHHMM.csv";
    }
}
