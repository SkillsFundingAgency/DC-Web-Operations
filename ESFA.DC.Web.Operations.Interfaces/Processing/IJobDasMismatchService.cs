using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobDasMismatchService
    {
        Task<string> GetDasMismatches(CancellationToken cancellationToken = default);
    }
}
