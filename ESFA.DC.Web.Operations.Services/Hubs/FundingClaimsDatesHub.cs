using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class FundingClaimsDatesHub : Hub
    {
        private readonly IHubContext<FundingClaimsDatesHub> _hubContext;
        private readonly IFundingClaimsDatesService _fundingClaimsDatesService;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FundingClaimsDatesHub(
            IHubContext<FundingClaimsDatesHub> hubContext,
            IFundingClaimsDatesService fundingClaimsDatesService,
            IDateTimeProvider dateTimeProvider)
        {
            _hubContext = hubContext;
            _fundingClaimsDatesService = fundingClaimsDatesService;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IEnumerable<FundingClaimsCollectionMetaData>> GetFundingClaimsCollectionMetaData()
        {
            return await _fundingClaimsDatesService.GetFundingClaimsCollectionMetaDataAsync();
        }

        public async Task<bool> UpdateFundingClaimsCollectionMetaData(FundingClaimsCollectionMetaData fundingClaimsCollectionMeta)
        {
            fundingClaimsCollectionMeta.DateTimeUpdatedUtc = _dateTimeProvider.GetNowUtc();
            return await _fundingClaimsDatesService.UpdateFundingClaimsCollectionMetaDataAsync(fundingClaimsCollectionMeta);
            //FundingClaimsCollectionMetaData funding = (FundingClaimsCollectionMetaData)fundingClaimsCollectionMeta;
            //return await _fundingClaimsDatesService.UpdateFundingClaimsCollectionMetaDataAsync(new FundingClaimsCollectionMetaData());
        }
    }
}
