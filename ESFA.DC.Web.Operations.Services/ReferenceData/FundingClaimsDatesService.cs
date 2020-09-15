using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Models.FundingClaimsDates;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.ReferenceData
{
    public class FundingClaimsDatesService : IFundingClaimsDatesService
    {
        private readonly ICollectionsService _collectionsService;
        private readonly IHttpClientService _httpClientService;
        private readonly string _baseUrl;

        public FundingClaimsDatesService(
            ApiSettings apiSettings,
            ICollectionsService collectionsService,
            IHttpClientService httpClientService)
        {
            _collectionsService = collectionsService;
            _httpClientService = httpClientService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
            _baseUrl = apiSettings.FundingClaimsApiBaseUrl;
        }

        public async Task<FundingClaimsCollectionMetaDataLastUpdate> GetLastUpdatedFundingClaimsCollectionMetaDataAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = await GetDataAsync($"{_baseUrl}/collection/lastupdate", cancellationToken);

             var collection = data == null ? null : _jsonSerializationService.Deserialize<FundingClaimsCollection>(data);

             if (collection != null)
             {
                return new FundingClaimsCollectionMetaDataLastUpdate()
                {
                    CreatedBy = collection.UpdatedBy,
                    DateTimeUpdatedUtc = collection.DateTimeUpdatedUtc,
                };
             }
            return await _httpClientService.GetAsync<FundingClaimsCollectionMetaDataLastUpdate>($"{_baseUrl}/api/fundingclaims-collection-metadata/lastupdate", cancellationToken);
        }

             return null;
        }

        public async Task<FundingClaimsDatesModel> GetFundingClaimsCollectionMetaDataAsync(
            int collectionYear,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var fundingClaimsCollectionMeta =
                _jsonSerializationService.Deserialize<IEnumerable<FundingClaimsCollection>>(
                    await GetDataAsync($"{_baseUrl}/collection/collectionYear/{collectionYear}", cancellationToken));
            var fundingClaimsCollectionMeta = await _httpClientService.GetAsync<IEnumerable<Jobs.Model.FundingClaimsCollectionMetaData.FundingClaimsCollectionMetaData>>($"{_baseUrl}/api/fundingclaims-collection-metadata/collectionYear/{collectionYear}", cancellationToken);

            var fundingClaimsCollectionMetaData = new List<FundingClaimsCollectionMetaData>();

            foreach (var fccm in fundingClaimsCollectionMeta)
            {
                fundingClaimsCollectionMetaData.Add(new FundingClaimsCollectionMetaData()
                {
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
            var entity = new FundingClaimsCollection()
            {
                CollectionId = fundingClaimsCollectionMetaData.CollectionId,
                SubmissionOpenDateUtc = fundingClaimsCollectionMetaData.SubmissionOpenDateUtc,
                SubmissionCloseDateUtc = fundingClaimsCollectionMetaData.SubmissionCloseDateUtc,
                SignatureCloseDateUtc = fundingClaimsCollectionMetaData.SignatureCloseDateUtc,
                RequiresSignature = fundingClaimsCollectionMetaData.RequiresSignature == 'Y',
                HelpdeskOpenDateUtc = fundingClaimsCollectionMetaData.HelpdeskOpenDateUtc,
                DateTimeUpdatedUtc = fundingClaimsCollectionMetaData.DateTimeUpdatedUtc,
                UpdatedBy = fundingClaimsCollectionMetaData.UpdatedBy,
                CollectionName = fundingClaimsCollectionMetaData.CollectionName,
                CollectionYear = fundingClaimsCollectionMetaData.CollectionYear,
            };

            var response = await SendDataAsync($"{_baseUrl}/collection/update", entity, cancellationToken);
            var response = await _httpClientService.SendDataAsync($"{_baseUrl}/api/fundingclaims-collection-metadata/update", entity, cancellationToken);
            return true;
        }

        public async Task<FundingClaimsDatesModel> GetFundingClaimsCollectionMetaDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var fundingClaimsCollectionMeta =
                _jsonSerializationService.Deserialize<IEnumerable<FundingClaimsCollection>>(
                    await GetDataAsync($"{_baseUrl}/collection", cancellationToken));
            var fundingClaimsCollectionMeta = await _httpClientService.GetAsync<IEnumerable<Jobs.Model.FundingClaimsCollectionMetaData.FundingClaimsCollectionMetaData>>($"{_baseUrl}/api/fundingclaims-collection-metadata", cancellationToken);

            var fundingClaimsCollectionMetaData = new List<FundingClaimsCollectionMetaData>();

            foreach (var fccm in fundingClaimsCollectionMeta)
            {
                fundingClaimsCollectionMetaData.Add(new FundingClaimsCollectionMetaData()
                {
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
