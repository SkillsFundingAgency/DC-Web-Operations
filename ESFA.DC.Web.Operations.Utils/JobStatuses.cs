using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Utils
{
    public class JobStatuses
    {
        public const string JobProcessing = "Processing";
        public const string JobCompleted = "Job Completed";
        public const string JobRejected = "Job Rejected";
        public const string JobFailed = "Job Failed";

        public const int JobStatus_Completed = 4;
        public static readonly List<int> ProcessingStates = new List<int> { 1, 2, 3 };
        public static readonly List<int> FailedStates = new List<int> { 5, 6 };
    }
}