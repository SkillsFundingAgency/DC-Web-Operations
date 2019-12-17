using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class ReportsService : BaseHttpClientService, IReportsService
    {
        private readonly string _baseUrl;

        public ReportsService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(jsonSerializationService, httpClient)
        {
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<long> RunReport(string reportType, int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            string reportUrl = string.Empty;

            reportUrl = $"{_baseUrl}/reports/{reportType}/{collectionYear}/{collectionPeriod}";
            var response = await GetDataAsync(reportUrl, cancellationToken);
            long.TryParse(response, out var result);

            return result;
        }

        public async Task<IEnumerable<ReportDetails>> GetAllReportDetails(int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            // get all reports
            string reportsUrl = $"{_baseUrl}/api/period-end/reports/{collectionYear}/{collectionPeriod}";

            IEnumerable<ReportDetails> reportDetailsList = _jsonSerializationService.Deserialize<IEnumerable<ReportDetails>>(
                await GetDataAsync(reportsUrl, cancellationToken));

            return reportDetailsList;
        }

        public async Task<int> GetReportStatus(long? jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobId.HasValue)
            {
                string errorMessage = ErrorMessageHelper.CreateErrorMessage($"Missing 'jobId' parameter");
                throw new Exception(errorMessage);
            }

            string url = $"{_baseUrl}/api/job/{jobId}/status";
            var response = await GetDataAsync(url, cancellationToken);
            int.TryParse(response, out var result);

            return result;
        }
    }
}
