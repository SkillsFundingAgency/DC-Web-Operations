using System.Net.Http;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.ILR
{
    public class ILRHistoryService : AbstractHistoryService, IILRHistoryService
    {
        public ILRHistoryService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider)
            : base(routeFactory, jsonSerializationService, httpClient, dateTimeProvider, $"{apiSettings.JobManagementApiBaseUrl}/api/period-end-history-ilr")
        {
        }
    }
}