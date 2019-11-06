using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Models.Provider;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class ProviderSearchHub : Hub
    {
        private readonly IHubContext<ProviderSearchHub> _hubContext;
        private readonly IAddNewProviderService _providerSearchService;

        public ProviderSearchHub(IHubContext<ProviderSearchHub> hubContext, IAddNewProviderService providerSearchService)
        {
            _hubContext = hubContext;
            _providerSearchService = providerSearchService;
        }

        public async Task<IEnumerable<ProviderSearchResult>> ProviderSearch(string query)
        {
            return await _providerSearchService.GetProviderSearchResults(query);
        }
    }
}
