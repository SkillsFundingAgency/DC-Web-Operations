using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class ProviderSearchHub : Hub
    {
        private readonly IHubContext<ProviderSearchHub> _hubContext;

        public ProviderSearchHub(IHubContext<ProviderSearchHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<ProviderSearchResult>> ProviderSearch(string query)
        {
            var returnValue = new []
            {
                new ProviderSearchResult("Provider 1", 123, 456),
                new ProviderSearchResult("Provider 2", 891, 312),
                new ProviderSearchResult("Provider 3", 111, 222),
                new ProviderSearchResult("Another Provider 1", 222, 333),
                new ProviderSearchResult("A new Provider", 444, 555)
            };
            return returnValue;
        }
    }

    public class ProviderSearchResult
    {
        public ProviderSearchResult(string providerName, int ukprn, int upin)
        {
            ProviderName = providerName;
            UKPRN = ukprn;
            UPIN = upin;
        }

        public string ProviderName { get; set; }

        public int UKPRN { get; set; }

        public int UPIN { get; set; }
    }
}
