namespace ESFA.DC.Web.Operations.Models.JobsProcessing
{
    public class JobProcessingLookupModel
    {
        public string ProviderName { get; set; }

        public long Ukprn { get; set; }

        public int TimeTakenSecond { get; set; }

        public int DateDifferSecond { get; set; }

        public string TimeTaken { get; set; }

        public string AverageProcessingTime { get; set; }
    }
}
