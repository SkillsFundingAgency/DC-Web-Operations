using ESFA.DC.Web.Operations.Interfaces.Collections;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class OnsPostcodes : ICollection
    {
        public string CollectionName => "OnsPostcodes";

        public string ReportName => "ONSPD-ValidationReport";

        public string DisplayName => "ONS postcodes";

        public string HubName => "onsPostcodesHub";

        public string FileFormat => ".zip";
    }
}
