using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.Provider;

namespace ESFA.DC.Web.Operations.Interfaces.Provider
{
    public interface ISearchProviderService
    {

        Task<IEnumerable<ProviderSearchResult>> GetNewProviderSearchResults(string query, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ProviderSearchResult>> GetExistingProviderSearchResults(string query, CancellationToken cancellationToken = default(CancellationToken));

    }
}
