using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.PeriodEnd;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models
{
    public class HistoryViewModel
    {
        public int Year { get; set; }

        public IEnumerable<int> CollectionYears { get; set; }

        public IEnumerable<ALLFHistoryDetail> PeriodHistories { get; set; }
    }
}
