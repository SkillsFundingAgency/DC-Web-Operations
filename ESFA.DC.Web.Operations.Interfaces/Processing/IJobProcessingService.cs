using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.JobsProcessing;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobProcessingService
    {
        Task<JobsProcessingModel> GetJobsThatAreProcessing(CancellationToken cancellationToken = default);
    }
}
