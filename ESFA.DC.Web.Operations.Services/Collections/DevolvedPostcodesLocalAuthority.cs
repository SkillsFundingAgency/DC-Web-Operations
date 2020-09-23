using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class DevolvedPostcodesLocalAuthority : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.DevolvedPostcodesLocalAuthority;

        public string ReportName => ReportTypes.DevolvedPostcodesLocalAuthorityReportName;

        public string DisplayName => "Devolved Postcodes";

        public string HubName => string.Empty;

        public string FileFormat => FileNameExtensionConsts.CSV;

        public string FileNameFormat => "MCAGLA_LocalAuthority_RD-YYYYMMDDHHMM.csv";
    }
}
