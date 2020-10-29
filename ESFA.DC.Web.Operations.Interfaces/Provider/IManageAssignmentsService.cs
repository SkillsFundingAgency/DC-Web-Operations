using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Interfaces.Provider
{
    public interface IManageAssignmentsService
    {
        Task<IEnumerable<CollectionAssignment>> GetAvailableCollectionsAsync(CancellationToken cancellationToken);

        Task<Models.Provider.Provider> GetProviderAsync(long ukprn, CancellationToken cancellationToken);

        Task<IEnumerable<CollectionAssignment>> GetActiveProviderAssignmentsAsync(long ukprn, List<CollectionAssignment> availableCollections, CancellationToken cancellationToken);

        Task<bool> UpdateProviderAssignmentsAsync(long ukprn, ICollection<CollectionAssignment> assignments, CancellationToken cancellationToken);
    }
}
