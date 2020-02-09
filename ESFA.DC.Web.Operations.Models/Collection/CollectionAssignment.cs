using System;

namespace ESFA.DC.Web.Operations.Models.Collection
{
    public class CollectionAssignment
    {
        public int CollectionId { get; set; }

        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int DisplayOrder { get; set; }

        public bool ToBeDeleted { get; set; }
    }
}
