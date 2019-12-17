using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IJobService
    {
        Task<long> SubmitJob(Models.Job.Job submittedJob, CancellationToken cancellationToken = default);

        Task<JobStatusType> GetJobStatus(long jobId, CancellationToken cancellationToken = default);
    }
}
