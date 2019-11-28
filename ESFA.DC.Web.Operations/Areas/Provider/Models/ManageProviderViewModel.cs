using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Areas.Provider.Models
{
    public class ManageProviderViewModel
    {
        public long Ukprn { get; set; }

        public string ProviderName { get; set; }

        public int Upin { get; set; }

        public bool IsMca { get; set; }

        public IEnumerable<CollectionAssignment> CollectionAssignments { get; set; }
    }
}
