using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IValidityPeriodService
    {
        Task<string> GetValidityPeriodList(int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> UpdateValidityPeriod(int collectionYear, int period, object validityPeriods, CancellationToken cancellationToken = default(CancellationToken));
    }
}