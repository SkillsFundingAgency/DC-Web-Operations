using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.Storage;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.ILR
{
    public class LearnerLevelViewReportingService
    {
        private readonly IStorageService _storageService;

        public LearnerLevelViewReportingService(
            IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<Stream> GetLLVReportFilesAsync(string containerName, string ukPrn, CancellationToken cancellationToken)
        {
            await _storageService.GetFile(containerName)
        }
    }
}