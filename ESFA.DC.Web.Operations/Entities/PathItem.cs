using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class PathItem
    {
        public PathItem()
        {
            PathItemJob = new HashSet<PathItemJob>();
        }

        public int PathItemId { get; set; }

        public int PathId { get; set; }

        public int Ordinal { get; set; }

        public virtual Path Path { get; set; }

        public virtual ICollection<PathItemJob> PathItemJob { get; set; }
    }
}
