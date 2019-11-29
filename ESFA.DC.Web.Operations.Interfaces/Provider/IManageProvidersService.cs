using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Interfaces.Provider
{
    public interface IManageProvidersService
    {
        Task<Models.Provider.Provider> GetProvider(long ukprn, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<CollectionAssignment>> GetProviderAssignments(long ukprn, CancellationToken cancellationToken = default(CancellationToken));
    }
}
