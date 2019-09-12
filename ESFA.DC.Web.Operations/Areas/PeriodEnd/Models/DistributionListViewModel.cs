using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public class DistributionListViewModel
    {
        public IEnumerable<RecipientGroupViewModel> RecipientGroups { get; set; }
    }
}