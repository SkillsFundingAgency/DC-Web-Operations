namespace ESFA.DC.Web.Operations.Topics.Data.Entities
{
    public partial class JobSubscriptionTask
    {
        public int JobTopicTaskId { get; set; }

        public int JobTopicId { get; set; }

        public string TaskName { get; set; }

        public short TaskOrder { get; set; }

        public bool? Enabled { get; set; }

        public virtual JobTopicSubscription JobTopic { get; set; }
    }
}
