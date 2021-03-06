﻿using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models.FundingClaimsDates;
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

        public async Task<FundingClaimsDatesModel> GetFundingClaimsCollectionMetaDataByYear(int year)
        {
            return await _fundingClaimsDatesService.GetFundingClaimsCollectionMetaDataAsync(year);
        }

        public async Task<FundingClaimsDatesModel> GetFundingClaimsCollectionMetaData()
        {
            return await _fundingClaimsDatesService.GetFundingClaimsCollectionMetaDataAsync();
        }

        public async Task<bool> UpdateFundingClaimsCollectionMetaData(FundingClaimsCollectionMetaData fundingClaimsCollectionMeta)
        {
            fundingClaimsCollectionMeta.DateTimeUpdatedUtc = _dateTimeProvider.GetNowUtc();
            return await _fundingClaimsDatesService.UpdateFundingClaimsCollectionMetaDataAsync(fundingClaimsCollectionMeta);
        }
    }
}
