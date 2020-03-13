using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Summarisation
{
    public class SummarisationTotal
    {
        public int CollectionReturnId { get; set; }

        public string CollectionType { get; set; }

        public string CollectionReturnCode { get; set; }

        public decimal TotalActualValue { get; set; }
    }
}
