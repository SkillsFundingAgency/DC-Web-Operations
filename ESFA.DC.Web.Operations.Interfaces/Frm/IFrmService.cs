using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.Frm
{
    public interface IFrmService
    {
        Task<long> RunValidation(string containerName, string folderKey, int periodNumber, string storageReference, CancellationToken cancellationToken = default(CancellationToken));

        Task<long> RunPublish(long jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> GetFrmStatus(long? jobId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
