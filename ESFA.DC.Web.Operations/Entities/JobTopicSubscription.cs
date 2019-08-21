using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class JobTopicSubscription
    {
        public JobTopicSubscription()
        {
            JobSubscriptionTask = new HashSet<JobSubscriptionTask>();
        }

        public int JobTopicId { get; set; }

        public int CollectionId { get; set; }

        public string TopicName { get; set; }

        public string SubscriptionName { get; set; }

        public short TopicOrder { get; set; }

        public bool? IsFirstStage { get; set; }

        public bool? Enabled { get; set; }

        public virtual Collection Collection { get; set; }

        public virtual ICollection<JobSubscriptionTask> JobSubscriptionTask { get; set; }
    }
}
