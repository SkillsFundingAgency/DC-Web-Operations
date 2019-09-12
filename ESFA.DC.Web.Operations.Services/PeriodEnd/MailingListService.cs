using System.Net.Http;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd
{
    public class MailingListService : BaseHttpClientService, IMailingListService
    {
        private readonly string _baseUrl;

        public MailingListService(
            IJsonSerializationService jsonSerializationService,
            HttpClient httpClient,
            ApiSettings apiSettings)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }
    }
}