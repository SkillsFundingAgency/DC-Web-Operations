using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class ProviderPostcodeSpecialistResources : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.ReferenceDataProviderPostcodeSpecialistResources;

        public string ReportName => ReportTypes.ProviderPostcodeSpecialistResourcesReportName;

        public string DisplayName => "Provider Postcode Specialist Resources";

        public string HubName => "providerPostcodeSpecialistResourcesHub";

        public string FileFormat => FileNameExtensionConsts.CSV;

        public string FileNameFormat => "ProviderPostcodeSpecialistResourceRD-YYYYMMDDHHMM.csv";
    }
}
