namespace ESFA.DC.Web.Operations.Entities
{
    public partial class JobEmailTemplate
    {
        public int Id { get; set; }

        public string TemplateOpenPeriod { get; set; }

        public string TemplateClosePeriod { get; set; }

        public short JobStatus { get; set; }

        public bool? Active { get; set; }

        public int CollectionId { get; set; }

        public virtual Collection Collection { get; set; }
    }
}
