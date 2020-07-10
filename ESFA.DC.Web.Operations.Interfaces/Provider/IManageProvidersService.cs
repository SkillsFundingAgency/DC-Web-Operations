using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Interfaces.Provider
{
    public interface IManageProvidersService
    {
        Task<Models.Provider.Provider> GetProviderAsync(long ukprn, CancellationToken cancellationToken);

        Task<IEnumerable<CollectionAssignment>> GetProviderAssignmentsAsync(long ukprn, CancellationToken cancellationToken);
    }
}
