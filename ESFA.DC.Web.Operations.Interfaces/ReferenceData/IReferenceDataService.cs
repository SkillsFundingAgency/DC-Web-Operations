using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Interfaces.ReferenceData
{
    public interface IReferenceDataService
    {
        Task SubmitJob(int period, string collectionName, string userName, string email, IFormFile file, CancellationToken cancellationToken);

        Task<ReferenceDataViewModel> GetSubmissionsPerCollectionAsync(
            string containerName,
            string collectionName,
            string reportName,
            int maxRows = Constants.MaxFilesToDisplay,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
