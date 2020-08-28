using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.ALLF
{
    public class ALLFHistoryService : AbstractHistoryService, IALLFHistoryService
    {
        private readonly IALLFPeriodEndService _periodEndService;

        public ALLFHistoryService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            IALLFPeriodEndService periodEndService,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider)
            : base(routeFactory, jsonSerializationService, httpClient, dateTimeProvider, $"{apiSettings.JobManagementApiBaseUrl}/api/period-end-history-allf")
        {
            _periodEndService = periodEndService;
        }

        public new Task<IEnumerable<FileUploadJobMetaDataModel>> GetHistoryDetails(int year, CancellationToken cancellationToken)
        {
            return _periodEndService.GetSubmissionsPerPeriodAsync(year, 0, cancellationToken);
        }
    }
}