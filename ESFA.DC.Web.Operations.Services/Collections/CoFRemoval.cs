using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class CoFRemoval : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.ReferenceDataConditionsOfFundingRemoval;

        public string ReportName => ReportTypes.ConditionOfFundingRemovalReportName;

        public string DisplayName => "Conditions of Funding Removal";

        public string HubName => "conditionOfFundingRemovalHub";

        public string FileFormat => FileNameExtensionConsts.CSV;

        public string FileNameFormat => "CoFRemovalRD-YYYYMMDDHHMM.csv";
    }
}
