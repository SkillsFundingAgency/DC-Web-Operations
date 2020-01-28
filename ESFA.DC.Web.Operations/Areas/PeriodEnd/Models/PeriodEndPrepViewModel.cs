using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public sealed class PeriodEndPrepViewModel
    {
        public int Period { get; set; }

        public int Year { get; set; }

        public bool IsCurrentPeriod { get; set; }

        public bool Closed { get; set; }

        public PeriodEndPrepModel PeriodEndPrepModel { get; set; }
    }
}