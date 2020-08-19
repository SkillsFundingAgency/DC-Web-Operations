using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Topics.Data.Auditing.Entities
{
    public partial class Audit
    {
        public int Id { get; set; }

        public string User { get; set; }

        public DateTime TimeStampUTC { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public int Differentiator { get; set; }
    }
}
