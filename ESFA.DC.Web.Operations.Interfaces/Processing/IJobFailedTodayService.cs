using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobFailedTodayService
    {
        Task<string> GetJobsThatAreFailedToday(CancellationToken cancellationToken = default);
    }
}
