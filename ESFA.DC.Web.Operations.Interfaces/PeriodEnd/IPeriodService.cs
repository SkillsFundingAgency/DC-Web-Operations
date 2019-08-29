using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.PeriodEnd;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IPeriodService
    {
        Task<PathYearPeriod> ReturnPeriod(CancellationToken cancellationToken = default(CancellationToken));
    }
}