using System;

namespace ESFA.DC.Web.Operations.ViewModels
{
    public class ReferenceDataJobViewModel
    {
        public string DisplayName { get; set; }

        public string Status { get; set; }

        public DateTime? NextRunDue { get; set; }
    }
}