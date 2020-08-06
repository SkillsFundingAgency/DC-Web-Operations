using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.FundingClaimsDates;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IFundingClaimsDatesService
    {
        Task<FundingClaimsCollectionMetaDataLastUpdate> GetLastUpdatedFundingClaimsCollectionMetaDataAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<FundingClaimsDatesModel> GetFundingClaimsCollectionMetaDataAsync(int collectionYear, CancellationToken cancellationToken = default(CancellationToken));

        Task<FundingClaimsDatesModel> GetFundingClaimsCollectionMetaDataAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> UpdateFundingClaimsCollectionMetaDataAsync(FundingClaimsCollectionMetaData fundingClaimsCollectionMeta, CancellationToken cancellationToken = default(CancellationToken));
    }
}
