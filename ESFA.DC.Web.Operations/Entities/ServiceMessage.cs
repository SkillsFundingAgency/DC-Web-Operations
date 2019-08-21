using System;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class ServiceMessage
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public bool Enabled { get; set; }

        public DateTime StartDateTimeUtc { get; set; }

        public DateTime? EndDateTimeUtc { get; set; }
    }
}
