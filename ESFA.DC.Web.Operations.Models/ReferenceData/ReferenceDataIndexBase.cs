using System;

namespace ESFA.DC.Web.Operations.Models.ReferenceData
{
    public class ReferenceDataIndexBase
    {
        public DateTime LastUpdatedDateTime { get; set; }

        public string LastUpdatedByWho { get; set; }

        public bool Valid { get; set; }
    }
}