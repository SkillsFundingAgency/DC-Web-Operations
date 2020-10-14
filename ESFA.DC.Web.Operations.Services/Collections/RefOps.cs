using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class RefOps : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.ReferenceDataOps;

        public string ReportName { get; }

        public string DisplayName { get; }

        public string HubName { get; }

        public string FileFormat => Utils.FileNameExtensionConsts.CSV;

        public string FileNameFormat => "PROVIDERS-YYYMMDD-HHMMSS.CSV";

        public override bool IsDisplayedOnReferenceDataSummary => false;
    }
}
