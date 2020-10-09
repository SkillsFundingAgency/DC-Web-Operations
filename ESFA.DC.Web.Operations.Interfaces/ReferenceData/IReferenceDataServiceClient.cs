using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces.ReferenceData
{
    public interface IReferenceDataServiceClient
    {
        Task<bool> IsReferenceDataCollectionExpired(string collectionName, CancellationToken cancellationToken);

        Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFilesPerCollectionAsync(string api, string collectionName, CancellationToken cancellationToken);
    }
}
