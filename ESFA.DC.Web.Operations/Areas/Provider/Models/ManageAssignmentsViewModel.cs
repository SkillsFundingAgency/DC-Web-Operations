using System;
using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Areas.Provider.Models
{
    public class ManageAssignmentsViewModel
    {
        public ManageAssignmentsViewModel()
        {
            ActiveCollectionsAssignments = new List<CollectionAssignment>();
            InactiveCollectionAssignments = new List<CollectionAssignment>();
        }

        public long Ukprn { get; set; }

        public string ProviderName { get; set; }

        public IList<CollectionAssignment> ActiveCollectionsAssignments { get; set; }

        public IList<CollectionAssignment> InactiveCollectionAssignments { get; set; }
    }
}
