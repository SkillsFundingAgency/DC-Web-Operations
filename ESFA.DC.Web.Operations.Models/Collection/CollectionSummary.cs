using System;

namespace ESFA.DC.Web.Operations.Models.Collection
{
    public class CollectionSummary
    {
        public int CollectionId { get; set; }

        public int CollectionYear { get; set; }

        public string CollectionName { get; set; }

        public bool IsCollectionEnabled { get; set; }

        public string CollectionCurrentOrLastPeriod { get; set; }

        public DateTime? CollectionClosedDate { get; set; }
    }
}
