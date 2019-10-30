namespace ESFA.DC.Web.Operations.Models
{
    public class PeriodEndState
    {
        public bool ReferenceDataJobsPaused { get; set; }

        public bool CollectionClosedEmailSent { get; set; }

        public bool PeriodEndStarted { get; set; }

        public bool McaReportsReady { get; set; }

        public bool McaReportsPublished { get; set; }

        public bool ProviderReportsReady { get; set; }

        public bool ProviderReportsPublished { get; set; }

        public bool PeriodEndClosed { get; set; }
    }
}