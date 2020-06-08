using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Areas.ReferenceData.Models
{
    public class ReferenceDataIndexBase
    {
        public DateTime LastUpdatedDateTime { get; set; }

        public string LastUpdatedByWho { get; set; }
    }
}
