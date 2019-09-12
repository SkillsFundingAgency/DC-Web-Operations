namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public class PeriodEndViewModel
    {
        public string Paths { get; set; }

        public int CurrentPeriod { get; set; }

        public int Period { get; set; }

        public int Year { get; set; }

        public bool ReportsPublished { get; set; }

        public bool Closed { get; set; }
    }
}