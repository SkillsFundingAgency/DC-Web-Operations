using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.PeriodEnd.ALLF
{
    public class ALLFPeriodEndService : BaseHttpClientService, IALLFPeriodEndService
    {
        private const string Api = "/api/period-end-allf/";
        private readonly string _baseUrl;

        public ALLFPeriodEndService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFilesPerPeriodAsync(int collectionYear, int period, CancellationToken cancellationToken)
        {
            string url = $"{_baseUrl}{Api}file-uploads/{collectionYear}/{period}";

            var data = await GetAsync<IEnumerable<FileUploadJobMetaDataModel>>(url, cancellationToken);

            return data;
        }
    }
}