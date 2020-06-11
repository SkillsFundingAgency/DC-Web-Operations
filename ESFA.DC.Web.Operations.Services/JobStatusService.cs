using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services
{
    public class JobStatusService : IJobStatusService
    {
        public string GetDisplayStatusFromJobStatus(FileUploadJobMetaDataModel model)
        {
            if (JobStatuses.ProcessingStates.Contains(model.JobStatus))
            {
                return JobStatuses.JobProcessing;
            }

            if (JobStatuses.FailedStates.Contains(model.JobStatus))
            {
                return JobStatuses.JobFailed;
            }

            if (model.JobStatus == JobStatuses.JobStatus_Completed)
            {
                return model.ErrorCount > 0 ? JobStatuses.JobRejected : JobStatuses.JobCompleted;
            }

            return string.Empty;
        }
    }
}