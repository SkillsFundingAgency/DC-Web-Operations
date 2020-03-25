using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndILR.Models
{
    public class HistoryViewModel
    {
        public int Year { get; set; }

        public IEnumerable<int> CollectionYears { get; set; }

        public IEnumerable<HistoryDetail> PeriodHistories { get; set; }
    }
}
