using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class DevolvedPostcodesPublication : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.DevolvedPostcodesPublication;

        public string ReportName => ReportTypes.DevolvedPostcodesPublicationSummaryReportName;

        public string DisplayName => "Devolved Postcodes Publication";

        public string HubName => "devolvedPostodesPublicationHub";

        public string FileFormat => FileNameExtensionConsts.ZIP;

        public string FileNameFormat => ReferenceDataOutputTypes.DevolvedPostcodesPublicationZipPreFix;
    }
}
