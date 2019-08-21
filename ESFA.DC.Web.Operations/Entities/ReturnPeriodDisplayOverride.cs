using System;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class ReturnPeriodDisplayOverride
    {
        public long Id { get; set; }

        public int ReturnPeriodId { get; set; }

        public DateTime? StartDateTimeUtc { get; set; }

        public DateTime? EndDateTimeUtc { get; set; }

        public int? PeriodNumber { get; set; }

        public virtual ReturnPeriod ReturnPeriod { get; set; }
    }
}
