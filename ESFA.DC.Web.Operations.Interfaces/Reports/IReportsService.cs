using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Models.Reports;

namespace ESFA.DC.Web.Operations.Interfaces.Reports
{
    public interface IReportsService
    {
        Task<long> RunReport(string reportType, int collectionYear, int collectionPeriod, string createdBy, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetAllReportDetails(int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> GetReportStatus(long? jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<IReport>> GetAvailableReportsAsync(int collectionYear, int period, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetOperationsReportsDetails(int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetFundingClaimsReportsDetails(CancellationToken cancellationToken = default(CancellationToken));

        string BuildFileName(
            ReportType reportType,
            int collectionYear,
            string periodString,
            string decodedFileName);
    }
}
