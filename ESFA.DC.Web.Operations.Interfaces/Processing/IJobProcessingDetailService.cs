using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.Detail;

namespace ESFA.DC.Web.Operations.Interfaces.Processing
{
    public interface IJobProcessingDetailService
    {
        Task<IEnumerable<JobDetails>> GetJobsProcessingDetails(DateTime startDateTimeUtc, DateTime endDateTimeUtc, CancellationToken cancellationToken = default);
    }
}
