using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Topics.Data.Entities
{
    public partial class Collection
    {
        public Collection()
        {
            JobTopicSubscription = new HashSet<JobTopicSubscription>();
        }

        public int CollectionId { get; set; }

        public string Name { get; set; }

        public bool IsOpen { get; set; }

        public int CollectionTypeId { get; set; }

        public int? CollectionYear { get; set; }

        public string Description { get; set; }

        public string SubText { get; set; }

        public bool? CrossloadingEnabled { get; set; }

        public bool? ProcessingOverrideFlag { get; set; }

        public bool MultiStageProcessing { get; set; }

        public string StorageReference { get; set; }

        public virtual CollectionType CollectionType { get; set; }

        public virtual ICollection<JobTopicSubscription> JobTopicSubscription { get; set; }
    }
}
