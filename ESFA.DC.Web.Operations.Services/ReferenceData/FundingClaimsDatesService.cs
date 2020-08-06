using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Models.FundingClaimsDates;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.ReferenceData
{
    public class FundingClaimsDatesService : BaseHttpClientService, IFundingClaimsDatesService
    {
        private readonly ICollectionsService _collectionsService;
        private readonly string _baseUrl;

        public FundingClaimsDatesService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            ICollectionsService collectionsService)
            : base(routeFactory, jsonSerializationService, httpClient)
        {
            _collectionsService = collectionsService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<FundingClaimsCollectionMetaDataLastUpdate> GetLastUpdatedFundingClaimsCollectionMetaDataAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = await GetDataAsync($"{_baseUrl}/api/fundingclaims-collection-metadata/lastupdate", cancellationToken);

            return data == null ? null : _jsonSerializationService.Deserialize<FundingClaimsCollectionMetaDataLastUpdate>(data);
        }

        public async Task<FundingClaimsDatesModel> GetFundingClaimsCollectionMetaDataAsync(
            int collectionYear,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var fundingClaimsCollectionMeta =
                _jsonSerializationService.Deserialize<IEnumerable<Jobs.Model.FundingClaimsCollectionMetaData.FundingClaimsCollectionMetaData>>(
                    await GetDataAsync($"{_baseUrl}/api/fundingclaims-collection-metadata/collectionYear/{collectionYear}", cancellationToken));

            var fundingClaimsCollectionMetaData = new List<FundingClaimsCollectionMetaData>();
            foreach (var fccm in fundingClaimsCollectionMeta)
            {
                fundingClaimsCollectionMetaData.Add(new FundingClaimsCollectionMetaData()
                {
                    Id = fccm.Id,
                    CollectionName = fccm.CollectionName,
                    CollectionId = fccm.CollectionId,
                    UpdatedBy = fccm.UpdatedBy,
                    CollectionYear = fccm.CollectionYear,
                    DateTimeUpdatedUtc = fccm.DateTimeUpdatedUtc,
                    HelpdeskOpenDateUtc = fccm.HelpdeskOpenDateUtc,
                    RequiresSignature = fccm.RequiresSignature.GetValueOrDefault() ? 'Y' : 'N',
                    SignatureCloseDateUtc = fccm.SignatureCloseDateUtc,
                    SubmissionCloseDateUtc = fccm.SubmissionCloseDateUtc,
                    SubmissionOpenDateUtc = fccm.SubmissionOpenDateUtc
                });
            }

            var collections = await _collectionsService.GetCollectionsByType(CollectionTypes.FundingClaim, cancellationToken);

            FundingClaimsDatesModel model = new FundingClaimsDatesModel()
            {
                FundingClaimsDatesList = fundingClaimsCollectionMetaData.OrderByDescending(x => x.CollectionId).ThenByDescending(x => x.SubmissionOpenDateUtc).ToList(),
                Collections = collections.Where(x => x.CollectionYear == collectionYear).ToList()
            };

            return model;
        }

        public async Task<bool> UpdateFundingClaimsCollectionMetaDataAsync(
            FundingClaimsCollectionMetaData fundingClaimsCollectionMetaData,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var entity = new Jobs.Model.FundingClaimsCollectionMetaData.FundingClaimsCollectionMetaData()
            {
                Id = fundingClaimsCollectionMetaData.Id,
                CollectionId = fundingClaimsCollectionMetaData.CollectionId,
                SubmissionOpenDateUtc = fundingClaimsCollectionMetaData.SubmissionOpenDateUtc,
                SubmissionCloseDateUtc = fundingClaimsCollectionMetaData.SubmissionCloseDateUtc,
                SignatureCloseDateUtc = fundingClaimsCollectionMetaData.SignatureCloseDateUtc,
                RequiresSignature = fundingClaimsCollectionMetaData.RequiresSignature == 'Y',
                HelpdeskOpenDateUtc = fundingClaimsCollectionMetaData.HelpdeskOpenDateUtc,
                DateTimeUpdatedUtc = fundingClaimsCollectionMetaData.DateTimeUpdatedUtc,
                UpdatedBy = fundingClaimsCollectionMetaData.UpdatedBy
            };

            var response = await SendDataAsync($"{_baseUrl}/api/fundingclaims-collection-metadata/update", entity, cancellationToken);
            return true;
        }

        public async Task<FundingClaimsDatesModel> GetFundingClaimsCollectionMetaDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var fundingClaimsCollectionMeta =
                _jsonSerializationService.Deserialize<IEnumerable<Jobs.Model.FundingClaimsCollectionMetaData.FundingClaimsCollectionMetaData>>(
                    await GetDataAsync($"{_baseUrl}/api/fundingclaims-collection-metadata", cancellationToken));

            var fundingClaimsCollectionMetaData = new List<FundingClaimsCollectionMetaData>();
            foreach (var fccm in fundingClaimsCollectionMeta)
            {
                fundingClaimsCollectionMetaData.Add(new FundingClaimsCollectionMetaData()
                {
                    Id = fccm.Id,
                    CollectionName = fccm.CollectionName,
                    CollectionId = fccm.CollectionId,
                    UpdatedBy = fccm.UpdatedBy,
                    CollectionYear = fccm.CollectionYear,
                    DateTimeUpdatedUtc = fccm.DateTimeUpdatedUtc,
                    HelpdeskOpenDateUtc = fccm.HelpdeskOpenDateUtc,
                    RequiresSignature = fccm.RequiresSignature.GetValueOrDefault() ? 'Y' : 'N',
                    SignatureCloseDateUtc = fccm.SignatureCloseDateUtc,
                    SubmissionCloseDateUtc = fccm.SubmissionCloseDateUtc,
                    SubmissionOpenDateUtc = fccm.SubmissionOpenDateUtc
                });
            }

            var collections = await _collectionsService.GetCollectionsByType(CollectionTypes.FundingClaim, cancellationToken);

            FundingClaimsDatesModel model = new FundingClaimsDatesModel()
            {
                FundingClaimsDatesList = fundingClaimsCollectionMetaData.OrderByDescending(x => x.CollectionId).ThenByDescending(x => x.SubmissionOpenDateUtc).ToList(),
                Collections = collections.ToList()
            };

            return model;
        }
    }
}
