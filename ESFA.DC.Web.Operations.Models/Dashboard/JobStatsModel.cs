using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.Dashboard.Job;

namespace ESFA.DC.Web.Operations.Models.Dashboard
{
    public sealed class JobStatsModel
    {
        public TodayStatsModel TodayStatsModel { get; set; }

        public SlowFilesComparedToThreePreviousModel SlowFilesComparedToThreePreviousModel { get; set; }

        public IEnumerable<JobsCurrentPeriodModel> JobsCurrentPeriodModels { get; set; }

        public ConcernsModel ConcernsModel { get; set; }
    }
}
