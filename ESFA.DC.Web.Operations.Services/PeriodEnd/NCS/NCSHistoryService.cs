using System.Net.Http;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.NCS
{
    public class NCSHistoryService : AbstractHistoryService, INCSHistoryService
    {
        public NCSHistoryService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, apiSettings, httpClient, $"{apiSettings.JobManagementApiBaseUrl}/api/period-end-history-ncs")
        {
        }
    }
}