using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobSubmittedService
    {
        Task<string> GetJobsThatAreSubmitted(CancellationToken cancellationToken = default);
    }
}
