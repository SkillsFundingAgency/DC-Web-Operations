using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobProcessingDetailService
    {
        Task<string> GetJobsProcessingDetailsForCurrentPeriod(short jobStatus, CancellationToken cancellationToken);

        Task<string> GetJobsProcessingDetailsForCurrentPeriodLast5Mins(short jobStatus, CancellationToken cancellationToken);

        Task<string> GetJobsProcessingDetailsForCurrentPeriodLastHour(short jobStatus, CancellationToken cancellationToken);
    }
}
