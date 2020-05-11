using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Interfaces.Provider
{
    public interface IManageAssignmentsService
    {
        Task<IEnumerable<CollectionAssignment>> GetAvailableCollections(CancellationToken cancellationToken = default(CancellationToken));

        Task<Models.Provider.Provider> GetProvider(long ukprn, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<CollectionAssignment>> GetProviderAssignments(long ukprn, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> UpdateProviderAssignments(long ukprn, ICollection<CollectionAssignment> assignments, CancellationToken cancellationToken = default(CancellationToken));
    }
}
