namespace ESFA.DC.Web.Operations.Areas.EmailDistribution.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ESFA.DC.EmailDistribution.Models;

    public class AddRemoveViewModel
    {
        public string Email { get; set; }

        public string AddRemove { get; set; }

        public List<RecipientGroup> Data { get; set; }
    }
}
