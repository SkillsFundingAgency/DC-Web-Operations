using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.ILR
{
    public class ILRHistoryService : AbstractHistoryService, IILRHistoryService
    {
        public ILRHistoryService(
            ApiSettings apiSettings,
            IHttpClientService httpClientService)
            : base($"{apiSettings.JobManagementApiBaseUrl}/api/period-end-history-ilr", httpClientService)
        {
        }
    }
}