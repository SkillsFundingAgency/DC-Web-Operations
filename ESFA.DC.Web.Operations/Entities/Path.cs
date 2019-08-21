using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class Path
    {
        public Path()
        {
            PathItem = new HashSet<PathItem>();
        }

        public int PathId { get; set; }

        public int PeriodEndId { get; set; }

        public int HubPathId { get; set; }

        public virtual PeriodEnd PeriodEnd { get; set; }

        public virtual ICollection<PathItem> PathItem { get; set; }
    }
}
