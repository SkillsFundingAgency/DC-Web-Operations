using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Provider
{
    public class ProviderAssignment
    {
        public long Ukprn { get; set; }

        public string CollectionName { get; set; }

        public DateTime CollectionStartDate { get; set; }

        public DateTime CollectionEndDate { get; set; }
    }
}
