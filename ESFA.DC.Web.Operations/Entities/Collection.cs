using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class Collection
    {
        public Collection()
        {
            Job = new HashSet<Job>();
            JobEmailTemplate = new HashSet<JobEmailTemplate>();
            JobTopicSubscription = new HashSet<JobTopicSubscription>();
            OrganisationCollection = new HashSet<OrganisationCollection>();
            ReturnPeriod = new HashSet<ReturnPeriod>();
            Schedule = new HashSet<Schedule>();
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

        public virtual ICollection<Job> Job { get; set; }

        public virtual ICollection<JobEmailTemplate> JobEmailTemplate { get; set; }

        public virtual ICollection<JobTopicSubscription> JobTopicSubscription { get; set; }

        public virtual ICollection<OrganisationCollection> OrganisationCollection { get; set; }

        public virtual ICollection<ReturnPeriod> ReturnPeriod { get; set; }

        public virtual ICollection<Schedule> Schedule { get; set; }
    }
}
