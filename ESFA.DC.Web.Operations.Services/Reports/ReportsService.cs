using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class ReportsService : BaseHttpClientService, IReportsService
    {
        private readonly string _baseUrl;
        private IDictionary<int, IEnumerable<string>> _collectionsByYear = new Dictionary<int, IEnumerable<string>>();
        private readonly ICollectionsService _collectionsService;

        public ReportsService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            ICollectionsService collectionsService)
            : base(routeFactory, jsonSerializationService, httpClient)
        {
            _collectionsService = collectionsService;
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<long> RunReport(string reportType, int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            long jobId = -1;

            if (!string.IsNullOrEmpty(reportType))
            {
                string collectionName;

                switch (reportType)
                {
                    case ReportTypes.PeriodEndDataQualityReport:
                        collectionName = Constants.PeriodEndDataQualityReportCollectionName;
                        break;

                    case ReportTypes.ProviderSubmissionsReport:
                        collectionName = Constants.ProviderSubmissionsReportCollectionName;
                        break;

                    case ReportTypes.PeriodEndMetricsReport:
                        collectionName = Constants.PeriodEndMetricsReportCollectionName;
                        break;

                    case ReportTypes.PeriodEndDataExtractReport:
                        collectionName = Constants.PeriodEndDataExtractReportCollectionName;
                        break;

                    case ReportTypes.InternalDataMatchReport:
                        collectionName = Constants.InternalDataMatchReportCollectionName;
                        break;

                    case ReportTypes.ActCountReport:
                        collectionName = Constants.ActCountReportCollectionName;
                        break;

                    default:
                        return -1;
                }

                var job = new FileUploadJob
                {
                    CollectionYear = collectionYear,
                    PeriodNumber = collectionPeriod,
                    CollectionName = collectionName.Replace(Constants.CollectionYearToken, collectionYear.ToString()),
                    StorageReference = Constants.ReportsBlobContainerName.Replace(Constants.CollectionYearToken, collectionYear.ToString()),
                    Status = Jobs.Model.Enums.JobStatusType.Ready,
                    JobId = 0
                };

                string url = $"{_baseUrl}/api/job";
                var json = _jsonSerializationService.Serialize(job);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                long.TryParse(result, out jobId);
            }

            return jobId;
        }

        public async Task<IEnumerable<ReportDetails>> GetAllReportDetails(int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<ReportDetails> reportDetailsList = null;
            var fileLocation = Constants.ReportsBlobContainerName.Replace(Constants.CollectionYearToken, collectionYear.ToString());
            var reportTypeCount = 3;

            // get all reports
            string reportsUrl = $"{_baseUrl}/api/period-end/reports/{collectionYear}/{collectionPeriod}/{fileLocation}/{reportTypeCount}";

            var file = await GetDataAsync(reportsUrl, cancellationToken);
            if (!string.IsNullOrEmpty(file))
            {
                reportDetailsList = _jsonSerializationService.Deserialize<IEnumerable<ReportDetails>>(file);
                StripLeaingReturnPeriodFromReportUrl(reportDetailsList);
            }

            return reportDetailsList;
        }

        public async Task<int> GetReportStatus(long? jobId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!jobId.HasValue)
            {
                throw new Exception("Missing 'jobId' parameter");
            }

            string url = $"{_baseUrl}/api/job/{jobId}/status";
            var response = await GetDataAsync(url, cancellationToken);
            int.TryParse(response, out var result);

            return result;
        }

        public async Task<IEnumerable<IReport>> GetAvailableReportsAsync(int collectionYear, IEnumerable<IReport> authorisedReports, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<string> collectionsForYear = new List<string>();

            if (!_collectionsByYear.TryGetValue(collectionYear, out collectionsForYear))
            {
                _collectionsByYear.Add(collectionYear, (await _collectionsService.GetAllCollectionsForYear(collectionYear, cancellationToken)).Select(s => s.CollectionName));
            }

            return authorisedReports
                .Where(w => _collectionsByYear[collectionYear].Contains(string.Format(CultureInfo.CurrentCulture, w.CollectionName, collectionYear)));
        }

        private void StripLeaingReturnPeriodFromReportUrl(IEnumerable<ReportDetails> reportDetailsList)
        {
            if (reportDetailsList != null)
            {
                // remove the leading 'R01/' etc from the url
                var prefixLength = "R01/".Length;
                foreach (var reportDetail in reportDetailsList)
                {
                    if (reportDetail.Url != null && reportDetail.Url.Length > prefixLength)
                    {
                        reportDetail.Url = reportDetail.Url.Substring(prefixLength, reportDetail.Url.Length - prefixLength);
                    }
                }
            }
        }
    }
}
