using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.ALLF
{
    public class ALLFHistoryService : IALLFHistoryService
    {
        private readonly IALLFPeriodEndService _periodEndService;

        public ALLFHistoryService(
            IALLFPeriodEndService periodEndService)
        {
            _periodEndService = periodEndService;
        }

        public Task<IEnumerable<FileUploadJobMetaDataModel>> GetHistoryDetails(int currentYear, int currentPeriod, CancellationToken cancellationToken)
        {
            return _periodEndService.GetSubmissionsForAllPeriodsAsync(currentYear, currentPeriod, cancellationToken);
        }
    }
}