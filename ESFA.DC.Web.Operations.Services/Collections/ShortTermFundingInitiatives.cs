using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class ShortTermFundingInitiatives : ICollection
    {
        public string CollectionName => CollectionNames.ShortTermFundingInitiatives;

        public string ReportName => ReportTypes.ShortTermFundingInitiativesReportName;

        public string DisplayName => "Short Term Funding Initiatives";

        public string HubName => "shortTermFundingInitiativesHub";

        public string FileFormat => FileNameExtensionConsts.CSV;

        public string FileNameFormat => "ShortTermFundingInitiativesRD-YYYYMMDDHHMM.csv";
    }
}
