using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobConcernService
    {
        Task<string> GetJobsThatAreConcern(CancellationToken cancellationToken = default);
    }
}
