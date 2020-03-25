using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IValidityPeriodService
    {
        Task<string> GetValidityPeriodList(int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> UpdateValidityPeriod(int period, object validityPeriods, CancellationToken cancellationToken = default(CancellationToken));
    }
}