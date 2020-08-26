using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class ValidationMessages2021 : BaseCollection, ICollection
    {
        public override string CollectionName => CollectionNames.ReferenceDataValidationMessages2021;

        public string ReportName => ReportTypes.ValidationMessagesReportName;

        public string DisplayName => "Validation Messages 20-21";

        public string HubName => "validationMessages2021Hub";

        public string FileFormat => FileNameExtensionConsts.CSV;

        public string FileNameFormat => "ValidationMessages-YYYYMMDDHHMM.csv";
    }
}
