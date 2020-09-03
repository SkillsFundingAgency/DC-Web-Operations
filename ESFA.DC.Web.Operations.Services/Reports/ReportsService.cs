using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ESFA.DC.Web.Operations.Services.Reports
{
    public class ReportsService : BaseHttpClientService, IReportsService
    {
        private readonly string _baseUrl;
        private IDictionary<int, IEnumerable<string>> _collectionsByYear = new Dictionary<int, IEnumerable<string>>();
        private readonly ICollectionsService _collectionsService;
        private readonly IEnumerable<IReport> _reports;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _operationsFileService;

        public ReportsService(
            IRouteFactory routeFactory,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            ICollectionsService collectionsService,
            IEnumerable<IReport> reports,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor,
            IDateTimeProvider dateTimeProvider,
            IIndex<PersistenceStorageKeys, IFileService> operationsFileService)
            : base(routeFactory, jsonSerializationService, dateTimeProvider, httpClient)
        {
            _collectionsService = collectionsService;
            _reports = reports;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _operationsFileService = operationsFileService[PersistenceStorageKeys.OperationsAzureStorage];
            _baseUrl = apiSettings.JobManagementApiBaseUrl;
        }

        public async Task<long> RunReport(string reportName, int collectionYear, int collectionPeriod, string createdBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            long jobId = -1;

            var report = _reports.Single(x => x.ReportName.Equals(reportName));

            var job = new FileUploadJob
            {
                CollectionYear = collectionYear,
                PeriodNumber = collectionPeriod,
                CollectionName = report.CollectionName.Replace(Constants.CollectionYearToken, collectionYear.ToString()),
                StorageReference = report.ContainerName.Replace(Constants.CollectionYearToken, collectionYear.ToString()),
                Status = Jobs.Model.Enums.JobStatusType.Ready,
                JobId = 0,
                CreatedBy = createdBy
            };

            string url = $"{_baseUrl}/api/job";
            var json = _jsonSerializationService.Serialize(job);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            long.TryParse(result, out jobId);

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

        public async Task<IEnumerable<ReportDetails>> GetOperationsReportsDetails(int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            var periodString = $"R{collectionPeriod:D2}";
            List<ReportDetails> reportDetailsList = new List<ReportDetails>();
            var fileReferences = (await _operationsFileService.GetFileReferencesAsync(Constants.OpsReportsBlobContainerName, $"Reports/{collectionYear}/{periodString}", true, CancellationToken.None)).ToList();

            foreach (var report in _reports.Where(x => x.ReportType == ReportType.Operations))
            {
                foreach (var fileReference in fileReferences)
                {
                    if (fileReference.IndexOf(report.DisplayName, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        var lastIndexOf = fileReference.LastIndexOf("/") + 1;
                        var url = fileReference.Substring(lastIndexOf, fileReference.Length - lastIndexOf);
                        reportDetailsList.Add(new ReportDetails { DisplayName = report.DisplayName, Url = url });
                    }
                }
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

        public async Task<IEnumerable<IReport>> GetAvailableReportsAsync(int collectionYear, CancellationToken cancellationToken = default(CancellationToken))
        {
            var authorisedReports = new List<IReport>();

            foreach (var report in _reports)
            {
                if (await IsAuthorised(report))
                {
                    authorisedReports.Add(report);
                }
            }

            IEnumerable<string> collectionsForYear = new List<string>();

            if (!_collectionsByYear.TryGetValue(collectionYear, out collectionsForYear))
            {
                _collectionsByYear.Add(collectionYear, (await _collectionsService.GetAllCollectionsForYear(collectionYear, cancellationToken)).Select(s => s.CollectionName));
            }

            return authorisedReports
                .Where(w => _collectionsByYear[collectionYear].Contains(w.CollectionName.Replace(Constants.CollectionYearToken, collectionYear.ToString())) || w.ReportType == ReportType.Validation);
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

        private async Task<bool> IsAuthorised(IReport report)
        {
            return (await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, report.Policy)).Succeeded;
        }
    }
}
