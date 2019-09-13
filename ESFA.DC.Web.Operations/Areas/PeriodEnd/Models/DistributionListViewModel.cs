using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models.EmailDistribution;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public class DistributionListViewModel
    {
        public IEnumerable<RecipientGroup> RecipientGroups { get; set; }
    }
}