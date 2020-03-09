using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobProcessingDetailService
    {
        Task<string> GetJobsThatAreProcessing(CancellationToken cancellationToken = default);
    }
}
