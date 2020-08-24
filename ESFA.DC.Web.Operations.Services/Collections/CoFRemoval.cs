using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class CoFRemoval : ICollection
    {
        public string CollectionName => "CoFRemoval";

        public string ReportName => "CoFRemovalRD-ValidationReport";

        public string DisplayName => "Conditions of Funding Removal";

        public string HubName => "conditionOfFundingRemovalHub";

        public string FileFormat => FileNameExtensionConsts.CSV;
    }
}
