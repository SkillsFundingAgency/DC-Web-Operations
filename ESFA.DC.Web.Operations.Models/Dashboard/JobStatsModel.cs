namespace ESFA.DC.Web.Operations.Models.Dashboard
{
    public sealed class JobStatsModel
    {
        public string AverageProcessingTime { get; set; }

        public int JobsProcessing { get; set; }

        public int JobsQueued { get; set; }

        public int Submissions { get; set; }

        public int FailedToday { get; set; }

        public int SlowFiles { get; set; }

        public int Concerns { get; set; }
    }
}
