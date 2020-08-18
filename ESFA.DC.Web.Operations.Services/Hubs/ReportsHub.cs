using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Models.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class ReportsHub : Hub
    {
        private readonly IHubContext<ReportsHub> _hubContext;
        private readonly IValidationRulesService _validationRulesService;
        private readonly ICollectionsService _collectionsService;
        private readonly IPeriodService _periodService;
        private readonly IReportsService _reportsService;
        private readonly IEnumerable<IReport> _reports;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportsHub(
            IHubContext<ReportsHub> hubContext,
            IValidationRulesService validationRulesService,
            ICollectionsService collectionsService,
            IPeriodService periodService,
            IReportsService reportsService,
            IEnumerable<IReport> reports,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _validationRulesService = validationRulesService;
            _collectionsService = collectionsService;
            _periodService = periodService;
            _reportsService = reportsService;
            _reports = reports;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ReportDetails>> GetReports(int collectionYear, int collectionPeriod)
        {
            var reportDetails = await _reportsService.GetAllReportDetails(collectionYear, collectionPeriod);

            // Gets reports list too.
            var authorisedReports = new List<IReport>();

            foreach (var report in _reports)
            {
                if (await IsAuthorised(report))
                {
                    authorisedReports.Add(report);
                }
            }

            var availableReportsAsync = await _reportsService.GetAvailableReportsAsync(collectionYear, authorisedReports);

            return reportDetails.Where(x => !string.IsNullOrEmpty(x.Url));
        }

        public async Task<ReportsModel> GetReportDetails(int collectionYear, int collectionPeriod)
        {
            var reportDetails = await _reportsService.GetAllReportDetails(collectionYear, collectionPeriod);

            var authorisedReports = new List<IReport>();

            foreach (var report in _reports)
            {
                if (await IsAuthorised(report))
                {
                    authorisedReports.Add(report);
                }
            }

            var availableReports = await _reportsService.GetAvailableReportsAsync(collectionYear, authorisedReports);

            var reportUrlDetails = reportDetails.Where(x => !string.IsNullOrEmpty(x.Url));

            var model = new ReportsModel
            {
                AvailableReportsList = availableReports,
                ReportUrlDetails = reportUrlDetails
            };
            return model;
        }

        public async Task<IEnumerable<int>> GetCollectionYears()
        {
            var collectionYears = await _collectionsService.GetCollectionYearsByType(CollectionTypeConstants.Ilr);
            return collectionYears.OrderByDescending(x => x).ToList();
        }

        public async Task<IEnumerable<ValidationRule>> GetValidationRules(int year)
        {
            return await _validationRulesService.GetValidationRules(year);
        }

        private async Task<bool> IsAuthorised(IReport report)
        {
            return (await _authorizationService.AuthorizeAsync(_httpContextAccessor.HttpContext.User, report.Policy)).Succeeded;
        }
    }
}
