using System;
using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Areas.Provider.Models
{
    public class ManageAssignmentsViewModel
    {
        public ManageAssignmentsViewModel()
        {
            CollectionsAssignments = new List<CollectionAssignment>();
        }

        public long Ukprn { get; set; }

        public string ProviderName { get; set; }

        public IList<CollectionAssignment> CollectionsAssignments { get; set; }
    }
}
