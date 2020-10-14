using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class NcsOutcomeRates : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.NcsOutcomeRates;

        public string ReportName => ReportTypes.NcsOutcomeRatesReportName;

        public string DisplayName => "NCS Outcome Rates";

        public string HubName => "ncsOutcomeRatesHub";

        public string FileFormat => FileNameExtensionConsts.CSV;

        public string FileNameFormat => "NCSOutcomeRatesRD-YYYYMMDDHHMM.csv";
    }
}
