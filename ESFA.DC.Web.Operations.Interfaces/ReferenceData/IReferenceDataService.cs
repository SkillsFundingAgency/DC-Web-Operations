using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Interfaces.ReferenceData
{
    public interface IReferenceDataService
    {
        Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken);

        Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmissionsPerCollectionAsync(
            string containerName,
            string collectionName,
            string reportName,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
