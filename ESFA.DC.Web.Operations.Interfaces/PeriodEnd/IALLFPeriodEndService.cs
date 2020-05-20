using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IALLFPeriodEndService
    {
        Task<string> GetPathItemStatesAsync(int? year, int? period, string collectionType, CancellationToken cancellationToken);

        Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFilesPerPeriodAsync(int collectionYear, int period, CancellationToken cancellationToken);

        Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmissionsPerPeriodAsync(int year, int period, CancellationToken cancellationToken);

        Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken);
    }
}