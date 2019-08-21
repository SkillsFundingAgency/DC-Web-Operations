using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class PeriodEnd
    {
        public PeriodEnd()
        {
            Path = new HashSet<Path>();
        }

        public int PeriodEndId { get; set; }

        public int PeriodId { get; set; }

        public bool ReportsPublished { get; set; }

        public bool Closed { get; set; }

        public virtual ReturnPeriod Period { get; set; }

        public virtual ICollection<Path> Path { get; set; }
    }
}
