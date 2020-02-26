using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
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

        public ReportsHub(
            IHubContext<ReportsHub> hubContext,
            IValidationRulesService validationRulesService,
            ICollectionsService collectionsService,
            IPeriodService periodService,
            IReportsService reportsService)
        {
            _hubContext = hubContext;
            _validationRulesService = validationRulesService;
            _collectionsService = collectionsService;
            _periodService = periodService;
            _reportsService = reportsService;
        }

        public async Task<IEnumerable<ReportDetails>> GetReports(int collectionYear, int collectionPeriod)
        {
            var reportDetails = await _reportsService.GetAllReportDetails(collectionYear, collectionPeriod);
            return reportDetails.Where(x => !string.IsNullOrEmpty(x.Url));
        }

        public async Task<IEnumerable<int>> GetCollectionYears()
        {
            var collectionYears = await _collectionsService.GetCollectionYearsByType(CollectionTypeConstants.Ilr);
            return collectionYears.OrderByDescending(x => x).ToList();
        }

        public async Task<IEnumerable<string>> GetValidationRules(int year)
        {
            var validationRules = await _validationRulesService.GetValidationRules(year);
            return validationRules.OrderBy(x => x);
        }
    }
}
