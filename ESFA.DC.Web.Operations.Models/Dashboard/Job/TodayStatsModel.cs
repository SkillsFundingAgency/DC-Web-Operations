using System;

namespace ESFA.DC.Web.Operations.Models.Dashboard.Job
{
    public sealed class TodayStatsModel
    {
        public int AverageTimeToday { get; set; }

        public string AverageProcessingTime
        {
            get
            {
                TimeSpan t = TimeSpan.FromMilliseconds(AverageTimeToday);
                var extraMins = t.Hours * 60;
                return $"{(t.Minutes + extraMins):D2}m {t.Seconds:D2}s";
            }
        }

        public int JobsProcessing { get; set; }

        public int JobsQueued { get; set; }

        public int FailedToday { get; set; }

        public int SubmissionsToday { get; set; }

        public int SubmissionsLastHour { get; set; }

        public int SubmissionsLast5Minutes { get; set; }
    }
}
