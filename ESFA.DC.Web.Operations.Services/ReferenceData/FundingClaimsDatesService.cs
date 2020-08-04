using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models.FundingClaimsDates;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.ReferenceData
{
    public class FundingClaimsDatesService : BaseHttpClientService, IFundingClaimsDatesService
    {
        private readonly string _baseUrl;

        public FundingClaimsDatesService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(routeFactory, jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<FundingClaimsCollectionMetaDataLastUpdate> GetLastUpdatedFundingClaimsCollectionMetaDataAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = await GetDataAsync($"{_baseUrl}/api/fundingclaimscollectionmetadata/lastupdate", cancellationToken);

            return data == null ? null : _jsonSerializationService.Deserialize<FundingClaimsCollectionMetaDataLastUpdate>(data);
        }

        public async Task<IEnumerable<FundingClaimsCollectionMetaData>> GetFundingClaimsCollectionMetaDataAsync(
            int collectionYear,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var fundingClaimsCollectionMeta =
                _jsonSerializationService.Deserialize<IEnumerable<Jobs.Model.FundingClaimsCollectionMetaData.FundingClaimsCollectionMetaData>>(
                    await GetDataAsync($"{_baseUrl}/api/fundingclaimscollectionmetadata/collectionYear/{collectionYear}", cancellationToken));

            var results = new List<FundingClaimsCollectionMetaData>();
            foreach (var fccm in fundingClaimsCollectionMeta)
            {
                results.Add(new FundingClaimsCollectionMetaData()
                {
                    Id = fccm.Id,
                    CollectionName = fccm.CollectionName,
                    CollectionId = fccm.CollectionId,
                    CreatedBy = fccm.CreatedBy,
                    CollectionYear = fccm.CollectionYear,
                    DateTimeUpdatedUtc = fccm.DateTimeUpdatedUtc,
                    HelpdeskOpenDateUtc = fccm.HelpdeskOpenDateUtc,
                    RequiresSignature = fccm.RequiresSignature.GetValueOrDefault() ? 'Y' : 'N',
                    SignatureCloseDateUtc = fccm.SignatureCloseDateUtc,
                    SubmissionCloseDateUtc = fccm.SubmissionCloseDateUtc,
                    SubmissionOpenDateUtc = fccm.SubmissionOpenDateUtc
                });
            }

            return results.OrderByDescending(x => x.CollectionId).ThenByDescending(x => x.SubmissionOpenDateUtc);
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
                CreatedBy = fundingClaimsCollectionMetaData.CreatedBy
            };

            var response = await SendDataAsync($"{_baseUrl}/api/fundingclaimscollectionmetadata/update", entity, cancellationToken);
            return true;
        }

        public async Task<IEnumerable<FundingClaimsCollectionMetaData>> GetFundingClaimsCollectionMetaDataAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var fundingClaimsCollectionMeta =
                _jsonSerializationService.Deserialize<IEnumerable<Jobs.Model.FundingClaimsCollectionMetaData.FundingClaimsCollectionMetaData>>(
                    await GetDataAsync($"{_baseUrl}/api/fundingclaimscollectionmetadata", cancellationToken));

            var results = new List<FundingClaimsCollectionMetaData>();
            foreach (var fccm in fundingClaimsCollectionMeta)
            {
                results.Add(new FundingClaimsCollectionMetaData()
                {
                    Id = fccm.Id,
                    CollectionName = fccm.CollectionName,
                    CollectionId = fccm.CollectionId,
                    CreatedBy = fccm.CreatedBy,
                    CollectionYear = fccm.CollectionYear,
                    DateTimeUpdatedUtc = fccm.DateTimeUpdatedUtc,
                    HelpdeskOpenDateUtc = fccm.HelpdeskOpenDateUtc,
                    RequiresSignature = fccm.RequiresSignature.GetValueOrDefault() ? 'Y' : 'N',
                    SignatureCloseDateUtc = fccm.SignatureCloseDateUtc,
                    SubmissionCloseDateUtc = fccm.SubmissionCloseDateUtc,
                    SubmissionOpenDateUtc = fccm.SubmissionOpenDateUtc
                });
            }

            return results.OrderByDescending(x => x.CollectionId).ThenByDescending(x => x.SubmissionOpenDateUtc); ;
        }
    }
}
