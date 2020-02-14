using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Interfaces.Collections
{
    public interface ICollectionsService
    {
        Task<IEnumerable<int>> GetAvailableCollectionYears(CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<CollectionSummary>> GetAllCollectionSummariesForYear(int year, CancellationToken cancellationToken = default(CancellationToken));

        Task<CollectionsManagement.Models.Collection> GetCollectionById(int collectionId, CancellationToken cancellationToken = default(CancellationToken));

        Task<CollectionsManagement.Models.Collection> GetCollectionAsync(string collectionName, CancellationToken cancellationToken = default(CancellationToken));

        Task<CollectionsManagement.Models.Collection> GetCollectionFromName(string collectionName, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReturnPeriod>> GetReturnPeriodsForCollection(int collectionId, CancellationToken cancellationToken = default(CancellationToken));

        Task<ReturnPeriod> GetReturnPeriod(int id, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> UpdateReturnPeriod(ReturnPeriod returnPeriod, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> SetCollectionProcessingOverride(int collectionId, bool? collectionProcessingOverrideStatus, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<FileUploadJob>> GetCollectionJobs(string collectionName, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> FailJob(int jobId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
