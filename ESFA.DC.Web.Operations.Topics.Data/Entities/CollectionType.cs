using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Topics.Data.Entities
{
    public partial class CollectionType
    {
        public CollectionType()
        {
            Collection = new HashSet<Collection>();
        }

        public int CollectionTypeId { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public int ConcurrentExecutionCount { get; set; }

        public virtual ICollection<Collection> Collection { get; set; }
    }
}
