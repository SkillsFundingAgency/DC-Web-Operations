using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Collections
{
    public class ValidationMessages2021 : ICollection
    {
        public string CollectionName => "ILRValidationMessages2021";

        public string ReportName => "ValidationMessagesRD-ValidationReport";

        public string DisplayName => "Validation Messages 20-21";

        public string HubName => "validationMessages2021Hub";

        public string FileFormat => FileNameExtensionConsts.CSV;
    }
}
