using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Summarisation
{
    public class SummarisationCollectionReturnCode
    {
        public string CollectionType { get; set; }

        public string CollectionReturnCode { get; set; }

        public DateTime DateTime { get; set; }

        public int Id { get; set; }
    }
}
