using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.PeriodEnd;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IValidityPeriodService
    {
        Task<string> GetValidityPeriodList(int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> UpdateValidityPeriods(int collectionYear, int period, IEnumerable<ValidityPeriod> validityPeriods, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetValidityStructure(int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken));
    }
}