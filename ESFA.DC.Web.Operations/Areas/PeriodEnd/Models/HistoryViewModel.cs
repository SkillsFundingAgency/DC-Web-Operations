using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.PeriodEnd;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public class HistoryViewModel
    {
        public int Year { get; set; }

        public IEnumerable<int> CollectionYears { get; set; }

        public IEnumerable<HistoryDetails> PeriodHistories { get; set; }
    }
}
