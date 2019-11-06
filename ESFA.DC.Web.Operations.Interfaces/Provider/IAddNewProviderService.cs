using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Provider;

namespace ESFA.DC.Web.Operations.Interfaces.Provider
{
    public interface IAddNewProviderService
    {
        Task<IEnumerable<ProviderSearchResult>> GetProviderSearchResults(string query);

        Task<HttpRawResponse> AddProvider(Models.Provider.Provider provider, CancellationToken cancellationToken);
    }
}
