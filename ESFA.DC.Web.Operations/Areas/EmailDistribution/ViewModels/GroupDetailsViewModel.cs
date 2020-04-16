using System.Collections.Generic;
using ESFA.DC.EmailDistribution.Models;

namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.ViewModels
{
    public class GroupDetailsViewModel
    {
        public string GroupName { get; set; }

        public List<Recipient> Recipients { get; set; }
    }
}
