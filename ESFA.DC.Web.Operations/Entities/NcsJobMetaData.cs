using System;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class NcsJobMetaData
    {
        public long Id { get; set; }

        public long JobId { get; set; }

        public string ExternalJobId { get; set; }

        public string TouchpointId { get; set; }

        public DateTime ExternalTimestamp { get; set; }

        public string ReportFileName { get; set; }

        public virtual Job Job { get; set; }
    }
}
