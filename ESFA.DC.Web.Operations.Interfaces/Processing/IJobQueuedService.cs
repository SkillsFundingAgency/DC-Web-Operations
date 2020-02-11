using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobQueuedService
    {
        Task<string> GetJobsThatAreQueued(CancellationToken cancellationToken = default);
    }
}
