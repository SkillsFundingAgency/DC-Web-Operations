using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobSlowFileService
    {
        Task<string> GetJobsThatAreSlowFile(CancellationToken cancellationToken = default);
    }
}
