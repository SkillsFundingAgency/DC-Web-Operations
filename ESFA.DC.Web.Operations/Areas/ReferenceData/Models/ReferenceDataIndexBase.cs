using System;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Models
{
    public class ReferenceDataIndexBase
    {
        public DateTime LastUpdatedDateTime { get; set; }

        public string LastUpdatedByWho { get; set; }
    }
}