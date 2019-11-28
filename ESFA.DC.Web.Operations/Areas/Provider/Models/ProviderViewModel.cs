using System.ComponentModel.DataAnnotations;

namespace ESFA.DC.Web.Operations.Areas.Provider.Models
{
    public class ProviderViewModel
    {
        public bool IsSingleAddNewProviderChoice { get; set; }

        public string ProviderName { get; set; }

        public long? Ukprn { get; set; }

        public int? Upin { get; set; }

        public bool IsMca { get; set; }
    }
}
