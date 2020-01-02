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
        Task<IEnumerable<ProviderSearchResult>> GetNewProviderSearchAsync(string query, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ProviderSearchResult>> GetExistingProviderSearchAsync(string query, CancellationToken cancellationToken = default(CancellationToken));
    }
}
