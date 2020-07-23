using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IFundingClaimsDatesService
    {
        Task<IEnumerable<FundingClaimsCollectionMetaData>> GetFundingClaimsCollectionMetaDataAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
