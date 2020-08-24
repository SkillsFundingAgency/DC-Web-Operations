using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class CampusIdentifiers : ICollection
    {
        public string CollectionName => CollectionNames.ReferenceDataCampusIdentifiers;

        public string ReportName => ReportTypes.CampusIdentifiersReportName;

        public string DisplayName => "Campus Identifiers";

        public string HubName => "campusIdentifiersHub";

        public string FileFormat => FileNameExtensionConsts.CSV;
    }
}
