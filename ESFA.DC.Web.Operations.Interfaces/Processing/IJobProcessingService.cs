using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobProcessingService
    {
        Task<string> GetJobsThatAreProcessing(CancellationToken cancellationToken = default);
    }
}
