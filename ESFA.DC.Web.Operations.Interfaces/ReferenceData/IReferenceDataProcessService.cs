using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.ReferenceData;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Interfaces.ReferenceData
{
    public interface IReferenceDataProcessService
    {
        Task<ReferenceDataViewModel> GetProcessOutputsForCollectionAsync(
            string containerName,
            string collectionName,
            string reportName,
            string reportExtension,
            string fileNameFormat,
            string fileNameExtension,
            int submissionCount = Constants.DefaultSubmissionCount,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
