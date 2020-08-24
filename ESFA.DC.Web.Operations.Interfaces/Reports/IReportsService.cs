using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models;
using ESFA.DC.Web.Operations.Models.Reports;

namespace ESFA.DC.Web.Operations.Interfaces.Reports
{
    public interface IReportsService
    {
        Task<long> RunReport(string reportType, int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<ReportDetails>> GetAllReportDetails(int collectionYear, int collectionPeriod, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> GetReportStatus(long? jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<IReport>> GetAvailableReportsAsync(int collectionYear, CancellationToken cancellationToken = default(CancellationToken));
    }
}
