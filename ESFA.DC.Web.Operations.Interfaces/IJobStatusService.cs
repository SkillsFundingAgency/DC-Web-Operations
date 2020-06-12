using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface IJobStatusService
    {
        string GetDisplayStatusFromJobStatus(FileUploadJobMetaDataModel model);
    }
}