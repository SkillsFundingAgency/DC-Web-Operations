using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobFailedCurrentPeriodService
    {
        Task<string> GetJobsFailedCurrentPeriodAsync(int? calendarYear, CancellationToken cancellationToken);
    }
}
