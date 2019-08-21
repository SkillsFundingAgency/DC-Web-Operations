using System;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class IlrJobMetaData
    {
        public long Id { get; set; }

        public long JobId { get; set; }

        public DateTime DateTimeSubmittedUtc { get; set; }

        public virtual Job Job { get; set; }
    }
}
