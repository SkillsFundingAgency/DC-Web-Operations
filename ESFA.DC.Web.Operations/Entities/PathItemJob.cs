namespace ESFA.DC.Web.Operations.Entities
{
    public partial class PathItemJob
    {
        public long JobId { get; set; }

        public int PathItemId { get; set; }

        public virtual Job Job { get; set; }

        public virtual PathItem PathItem { get; set; }
    }
}
