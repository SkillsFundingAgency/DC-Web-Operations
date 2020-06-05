using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
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

        Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmissionsPerPeriodAsync(
            int year,
            int period,
            bool includeAll = false,
            CancellationToken cancellationToken = default(CancellationToken));

        Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken);
    }
}