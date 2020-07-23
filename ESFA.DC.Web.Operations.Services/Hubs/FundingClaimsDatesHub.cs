using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class FundingClaimsDatesHub : Hub
    {
        private readonly IHubContext<FundingClaimsDatesHub> _hubContext;
        private readonly IFundingClaimsDatesService _fundingClaimsDatesService;

        public FundingClaimsDatesHub(
            IHubContext<FundingClaimsDatesHub> hubContext,
            IFundingClaimsDatesService fundingClaimsDatesService)
        {
            _hubContext = hubContext;
            _fundingClaimsDatesService = fundingClaimsDatesService;
        }

        public async Task<IEnumerable<FundingClaimsCollectionMetaData>> GetFundingClaimsCollectionMetaData()
        {
            return await _fundingClaimsDatesService.GetFundingClaimsCollectionMetaDataAsync();
        }
    }
}
