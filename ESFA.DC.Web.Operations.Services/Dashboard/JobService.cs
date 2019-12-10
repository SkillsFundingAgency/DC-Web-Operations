using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager.Data;
using ESFA.DC.JobQueueManager.Data.Entities;
using ESFA.DC.Logging.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Dashboard;
using ESFA.DC.Web.Operations.Models.Dashboard;
using ESFA.DC.Web.Operations.Models.Dashboard.Job;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Web.Operations.Services.Dashboard
{
    public sealed class JobService : IJobService
    {
        private readonly Func<IJobQueueDataContext> _jobQueueDataContext;
        private readonly ConnectionStrings _connectionStrings;
        private readonly ILogger _logger;

        public JobService(ConnectionStrings connectionStrings, ILogger logger)
        {
            _connectionStrings = connectionStrings;
            _logger = logger;
        }

        public async Task<JobStatsModel> ProvideAsync(CancellationToken cancellationToken)
        {
            JobStatsModel jobStatsModel = new JobStatsModel
            {
                TodayStatsModel = new TodayStatsModel(),
                SlowFilesComparedToThreePreviousModel = new SlowFilesComparedToThreePreviousModel(),
                JobsCurrentPeriodModels = new List<JobsCurrentPeriodModel>(),
                ConcernsModel = new ConcernsModel()
            };

            try
            {
                using (var connection = new SqlConnection(_connectionStrings.JobManagement))
                {
                    await connection.OpenAsync(cancellationToken);

                    SqlMapper.GridReader results = await connection.QueryMultipleAsync(
                        "[dbo].[Dashboard]",
                        commandType: CommandType.StoredProcedure);

                    jobStatsModel.TodayStatsModel = (await results.ReadAsync<TodayStatsModel>()).Single();
                    jobStatsModel.SlowFilesComparedToThreePreviousModel =
                        (await results.ReadAsync<SlowFilesComparedToThreePreviousModel>()).Single();
                    jobStatsModel.JobsCurrentPeriodModels = await results.ReadAsync<JobsCurrentPeriodModel>();
                    jobStatsModel.ConcernsModel = (await results.ReadAsync<ConcernsModel>()).Single();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get dashboard data", ex);
            }

            return jobStatsModel;
        }
    }
}
