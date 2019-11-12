namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public class PeriodEndPrepViewModel
    {
        public int Period { get; set; }

        public int Year { get; set; }

        public string ReferenceDataJobs { get; set; }

        public string FailedJobs { get; set; }

        public bool Closed { get; set; }

        public bool CollectionClosedEmailSent { get; set; }

        public bool IsCurrentPeriod { get; set; }
    }
}