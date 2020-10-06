using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class FisReferenceData2021 : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.FisReferenceData2021;

        public string ReportName => ReportTypes.FisReferenceData2021SummaryReportName;

        public string DisplayName => "FIS reference data 2021";

        public string HubName => "fisReferenceData2021Hub";

        public string FileFormat => FileNameExtensionConsts.ZIP;

        public string FileNameFormat => ReferenceDataOutputTypes.FisReferenceData2021ZipPreFix;
    }
}
