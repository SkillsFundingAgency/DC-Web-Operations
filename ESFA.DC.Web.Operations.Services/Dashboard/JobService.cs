using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Models.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Web.Operations.Services.Dashboard
{
    public sealed class JobService : IJobService
    {
        private readonly Func<IJobQueueDataContext> _jobQueueDataContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly int[] processingStatus = { 2, 3 };
        private readonly int[] failedStatus = { 5, 6 };

        private readonly string slowFilesSql = "Select Count(average.Ukprn) Cnt from (\r\nSELECT [Ukprn],     \r\n       AVG(DATEDIFF([SECOND], ijmd.DateTimeSubmittedUtc, j.DateTimeUpdatedUtc)) [TimeTaken]\r\n  FROM [dbo].[Job] j\r\n  inner join [dbo].[IlrJobMetaData] ijmd on ijmd.JobId = j.JobId\r\nWhere Status = 4\r\nGroup by Ukprn\r\n) average\r\ninner join [dbo].[job] j on j.Ukprn = average.Ukprn\r\ninner join [dbo].[IlrJobMetaData] ijmd on ijmd.JobId = j.JobId\r\nwhere DATEDIFF([SECOND], ijmd.DateTimeSubmittedUtc, j.DateTimeUpdatedUtc) > average.TimeTaken * 1.2 and Status in (2,3)";

        public JobService(Func<IJobQueueDataContext> jobQueueDataContext, IDateTimeProvider dateTimeProvider)
        {
            _jobQueueDataContext = jobQueueDataContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<JobStatsModel> ProvideAsync(CancellationToken cancellationToken)
        {
            DateTime nowUtc = _dateTimeProvider.GetNowUtc();
            nowUtc = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day);
            JobStatsModel jobStatsModel = new JobStatsModel();
            double average;
            int jobsProcessing, jobsQueued, submissions, failedToday;
            IList<SlowFileCountModel> slowFiles;
            IList<LatestFailedJobPerCollectionPerPeriodModel> latestFailedJobPerCollectionPerPeriodModels = null;

            using (var context = _jobQueueDataContext())
            {
                average = await context.Job
                    .Where(x => x.Collection.CollectionTypeId != 4 &&
                                x.DateTimeUpdatedUtc.HasValue &&
                                x.DateTimeCreatedUtc > nowUtc &&
                                (x.Collection.CollectionTypeId != 1 || (x.Collection.CollectionTypeId == 1 && x.IlrJobMetaData.Any())))
                    .Select(x => (x.DateTimeUpdatedUtc.Value - x.DateTimeCreatedUtc).TotalMilliseconds)
                    .DefaultIfEmpty(0)
                    .AverageAsync(x => x, cancellationToken);

                jobsProcessing = await context.Job
                    .Where(x => x.Collection.CollectionTypeId != 4 &&
                                x.DateTimeCreatedUtc > nowUtc &&
                                (x.Collection.CollectionTypeId != 1 || (x.Collection.CollectionTypeId == 1 && x.IlrJobMetaData.Any())) &&
                                processingStatus.Contains(x.Status))
                    .CountAsync(cancellationToken);

                jobsQueued = await context.Job
                    .Where(x => x.Collection.CollectionTypeId != 4 &&
                                x.DateTimeCreatedUtc > nowUtc &&
                                (x.Collection.CollectionTypeId != 1 || (x.Collection.CollectionTypeId == 1 && x.IlrJobMetaData.Any())) &&
                                x.Status == 1)
                    .CountAsync(cancellationToken);

                submissions = await context.Job
                    .Where(x => x.Collection.CollectionTypeId != 4 &&
                                x.DateTimeCreatedUtc > nowUtc &&
                                (x.Collection.CollectionTypeId != 1 || (x.Collection.CollectionTypeId == 1 && x.IlrJobMetaData.Any())))
                    .CountAsync(cancellationToken);

                failedToday = await context.Job
                    .Where(x => x.Collection.CollectionTypeId != 4 &&
                                x.DateTimeCreatedUtc > nowUtc &&
                                (x.Collection.CollectionTypeId != 1 || (x.Collection.CollectionTypeId == 1 && x.IlrJobMetaData.Any())) &&
                                failedStatus.Contains(x.Status))
                    .CountAsync(cancellationToken);

                slowFiles = await context.FromSqlAsync<SlowFileCountModel>(CommandType.Text, slowFilesSql, null);

                ReturnPeriod currentPeriod = await context.ReturnPeriod.Include(x => x.Collection).SingleOrDefaultAsync(x => x.StartDateTimeUtc >= nowUtc && x.EndDateTimeUtc <= nowUtc, cancellationToken);

                if (currentPeriod != null)
                {
                    latestFailedJobPerCollectionPerPeriodModels = await context
                        .FromSqlAsync<LatestFailedJobPerCollectionPerPeriodModel>(
                            CommandType.StoredProcedure,
                            "[dbo].[GetLatestFailedJobsPerCollectionPerPeriod]",
                            new { collectionYear = currentPeriod.Collection, periodNumber = currentPeriod.PeriodNumber });
                }
            }

            TimeSpan t = TimeSpan.FromMilliseconds(average);
            var extraMins = t.Hours * 60;
            jobStatsModel.AverageProcessingTime = $"{(t.Minutes + extraMins):D2}m {t.Seconds:D2}s";
            jobStatsModel.JobsProcessing = jobsProcessing;
            jobStatsModel.JobsQueued = jobsQueued;
            jobStatsModel.Submissions = submissions;
            jobStatsModel.FailedToday = failedToday;
            jobStatsModel.SlowFiles = slowFiles.Any() ? slowFiles[0].Cnt : 0;
            jobStatsModel.Concerns = latestFailedJobPerCollectionPerPeriodModels?.Count ?? 0;

            return jobStatsModel;
        }
    }
}
