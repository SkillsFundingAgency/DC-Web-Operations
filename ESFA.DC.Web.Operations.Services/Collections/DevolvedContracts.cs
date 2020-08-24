using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class DevolvedContracts : ICollection
    {
        public string CollectionName => CollectionNames.DevolvedContracts;

        public string ReportName => ReportTypes.DevolvedContractsReportName;

        public string DisplayName => "Devolved Contracts";

        public string HubName => "devolvedContractsHub";

        public string FileFormat => FileNameExtensionConsts.CSV;
    }
}
