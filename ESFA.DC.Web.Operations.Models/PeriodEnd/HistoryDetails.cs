using System;

namespace ESFA.DC.Web.Operations.Models.PeriodEnd
{
    public class HistoryDetails
    {
        public int Period { get; set; }

        public int Year { get; set; }

        public DateTime? PeriodEndStart { get; set; }

        public DateTime? PeriodEndFinish { get; set; }
    }
}