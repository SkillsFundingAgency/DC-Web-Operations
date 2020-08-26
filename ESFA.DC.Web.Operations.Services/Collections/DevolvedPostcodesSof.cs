using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class DevolvedPostcodesSof : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.DevolvedPostcodesSof;

        public string ReportName => ReportTypes.DevolvedPostcodesSofReportName;

        public string DisplayName => "Devolved Postcodes";

        public string HubName => string.Empty;

        public string FileFormat => FileNameExtensionConsts.CSV;

        public string FileNameFormat => "MCAGLA_SOF_RD-YYYYMMDDHHMM.csv";
    }
}
