using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class ProviderPostcodeSpecialistResources : ICollection
    {
        public string CollectionName => "ProviderPostcodeSpecialistResources";

        public string ReportName => "ProviderPostcodeSpecialistResourcesRD-ValidationReport";

        public string DisplayName => "Provider Postcode Specialist Resources";

        public string HubName => "providerPostcodeSpecialistResourcesHub";

        public string FileFormat => FileNameExtensionConsts.CSV;
    }
}
