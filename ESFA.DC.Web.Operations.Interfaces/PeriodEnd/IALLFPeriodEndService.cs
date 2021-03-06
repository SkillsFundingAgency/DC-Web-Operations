﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.ALLF;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IALLFPeriodEndService
    {
        Task InitialisePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken);

        Task StartPeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken);

        Task ProceedAsync(int year, int period, int path = 0, CancellationToken cancellationToken = default(CancellationToken));

        Task ClosePeriodEndAsync(int year, int period, string collectionType, CancellationToken cancellationToken);

        Task ReSubmitFailedJobAsync(long jobId, CancellationToken cancellationToken);

        Task<string> GetPathItemStatesAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken);

        Task<PeriodEndViewModel> GetPathState(int? collectionYear, int? period, CancellationToken cancellationToken);

        Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmissionsPerPeriodAsync(
            int year,
            int period,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmissionsForAllPeriodsAsync(int year, int latestPeriod, CancellationToken cancellationToken = default(CancellationToken));

        Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken);
    }
}