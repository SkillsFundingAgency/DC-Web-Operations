using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Reports;
using ESFA.DC.Web.Operations.Interfaces.ValidationRules;
using ESFA.DC.Web.Operations.Models.Reports;
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

        public async Task<ReportsModel> GetReportDetails(int collectionYear, int collectionPeriod)
        {
            var reportDetails = await _reportsService.GetAllReportDetails(collectionYear, collectionPeriod);
            var operationsReportsDetails = await _reportsService.GetOperationsReportsDetails(collectionYear, collectionPeriod);
            var fundingClaimsReportsDetails = await _reportsService.GetFundingClaimsReportsDetails();

            var reportDetailsList = reportDetails.ToList();
            reportDetailsList.AddRange(operationsReportsDetails);
            reportDetailsList.AddRange(fundingClaimsReportsDetails);
            var reportUrlDetails = reportDetailsList.Where(x => !string.IsNullOrEmpty(x.Url));

            var availableReports = await _reportsService.GetAvailableReportsAsync(collectionYear);
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
    }
}
