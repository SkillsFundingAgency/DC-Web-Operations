using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Interfaces.ReferenceData
{
    public interface IReferenceDataService
    {
        Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken);
    }
}
