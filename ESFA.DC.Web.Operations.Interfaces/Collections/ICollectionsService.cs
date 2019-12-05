using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Interfaces.Collections
{
    public interface ICollectionsService
    {
        Task<IEnumerable<int>> GetAvailableCollectionYears(CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<CollectionSummary>> GetAllCollectionSummariesForYear(int year, CancellationToken cancellationToken = default(CancellationToken));
        
        Task<Collection> GetCollectionById(int collectionId, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReturnPeriod>> GetReturnPeriodsForCollection(int collectionId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
