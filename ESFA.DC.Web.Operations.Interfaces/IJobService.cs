using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IJobService
    {
        Task<long> SubmitJob(Models.Job.JobSubmission submittedJobSubmission, CancellationToken cancellationToken = default);

        Task<JobStatusType> GetJobStatus(long jobId, CancellationToken cancellationToken = default);

        Task<SubmittedJob> GetJob(long ukprn, long jobId, CancellationToken cancellationToken = default);

        Task<SubmittedJob> GetLatestJobForCollectionAsync(string collectionName, CancellationToken cancellationToken);
    }
}
