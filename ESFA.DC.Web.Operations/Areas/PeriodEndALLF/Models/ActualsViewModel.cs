using System;
using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.PeriodEnd;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models
{
    public class ActualsViewModel
    {
        public string CurrentReturn { get; set; }

        public DateTime OpenUntil { get; set; }

        public List<ALLFHistoryDetail> History { get; set; }
    }
}
