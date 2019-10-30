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

        public async Task<string[]> ProviderSearch(string query)
        {
            var returnValue = new[] { "Afghanistan", "Akrotiri", "Albania", "Algeria" };
            //await _hubContext.Clients.All.SendAsync("updateSearchResults", returnValue);
            return returnValue;
        }
    }
}
