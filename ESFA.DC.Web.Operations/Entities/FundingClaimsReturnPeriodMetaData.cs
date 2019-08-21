using System;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class FundingClaimsReturnPeriodMetaData
    {
        public int Id { get; set; }

        public int ReturnPeriodId { get; set; }

        public bool RequireSigntarure { get; set; }

        public DateTime? SignatureCloseDateTimeUtc { get; set; }

        public virtual ReturnPeriod ReturnPeriod { get; set; }
    }
}
