using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Provider;

namespace ESFA.DC.Web.Operations.Interfaces.Provider
{
    public interface IAddNewProviderService
    {
        Task<Models.Provider.Provider> GetProvider(long ukprn, CancellationToken cancellationToken = default(CancellationToken));

        Task<HttpRawResponse> AddProvider(Models.Provider.Provider provider, CancellationToken cancellationToken);
    }
}
