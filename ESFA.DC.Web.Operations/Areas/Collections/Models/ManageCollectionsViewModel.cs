using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Areas.Collections.Models
{
    public class ManageCollectionsViewModel
    {
        public IList<CollectionSummary> Collections { get; set; }
    }
}
