using System.ComponentModel.DataAnnotations;

namespace ESFA.DC.Web.Operations.Areas.Provider.Models
{
    public class ProviderViewModel
    {
        public bool IsSingleAddNewProviderChoice { get; set; }

        [Required(ErrorMessage = "The Provider Name field is mandatory")]
        [StringLength(250, ErrorMessage = "The maximum length of the Providers Name is 250 characters")]
        public string ProviderName { get; set; }

        [Required(ErrorMessage = "The UKPRN field is mandatory")]
        [Range(10000000, 99999999, ErrorMessage = "UKPRN must be 8 digits long")]
        public long? Ukprn { get; set; }

        [Range(100000, 999999, ErrorMessage = "UPIN must be 6 digits long")]
        public int? Upin { get; set; }

        public bool IsMca { get; set; }
    }
}
