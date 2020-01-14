using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Frm
{
    public interface IFrmService
    {
        Task<int> GetFrmStatus(long? jobId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
