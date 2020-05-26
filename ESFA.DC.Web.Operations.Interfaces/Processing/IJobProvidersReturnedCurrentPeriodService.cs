using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobProvidersReturnedCurrentPeriodService
    {
        Task<string> GetProvidersReturnedCurrentPeriodAsync(CancellationToken cancellationToken);
    }
}
