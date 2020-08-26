using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class OnsPostcodes : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.OnsPostcodes;

        public string ReportName => ReportTypes.OnsPostcodesReportName;

        public string DisplayName => "ONS postcodes";

        public string HubName => "onsPostcodesHub";

        public string FileFormat => FileNameExtensionConsts.ZIP;

        public string FileNameFormat => "ONSPD-YYYYMMDD.zip";
    }
}
