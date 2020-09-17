using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class FisReferenceData2021 : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.FisReferenceData2021;

        public string ReportName => string.Empty;

        public string DisplayName => "FIS reference data";

        public string HubName => "fisReferenceData2021Hub";

        public string FileFormat => string.Empty;

        public string FileNameFormat => string.Empty;
    }
}
