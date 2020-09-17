using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.NCS
{
    public class NCSHistoryService : AbstractHistoryService, INCSHistoryService
    {
        public NCSHistoryService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
            : base($"{apiSettings.JobManagementApiBaseUrl}/api/period-end-history-ncs", httpClientService)
        {
        }
    }
}