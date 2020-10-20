using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class DevolvedPostcodesFullName : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.DevolvedPostcodesFullName;

        public string ReportName => ReportTypes.DevolvedPostcodesFullNameReportName;

        public string DisplayName => "Devolved Postcodes";

        public string HubName => string.Empty;

        public string FileFormat => FileNameExtensionConsts.CSV;

        public string FileNameFormat => "MCAGLA_FullName_RD-YYYYMMDDHHMM.csv";

        public override bool IsDisplayedOnReferenceDataSummary => false;
    }
}
