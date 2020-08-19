using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing
{
   public class Audit<T>
    {
        public int Id { get; set; }

        public string User { get; set; }

        public DateTime TimeStampUTC { get; set; }

        public T OldValue { get; set; }

        public T NewValue { get; set; }

        public int Differentiator { get; set; }
    }
}
