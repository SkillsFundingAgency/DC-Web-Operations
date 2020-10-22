using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Enums;
using ESFA.DC.Jobs.Model.Processing.JobsProcessing;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Processing;
using ESFA.DC.Web.Operations.Models.Processing;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Services.Hubs
{
    public class JobProcessingDetailHub : Hub
    {
        private readonly IJobProcessingDetailService _jobProcessingDetailService;
        private readonly IJsonSerializationService _jsonSerializationService;

        public JobProcessingDetailHub(
            IJobProcessingDetailService jobProcessingDetailService,
            IJsonSerializationService jsonSerializationService)
        {
            _jobProcessingDetailService = jobProcessingDetailService;
            _jsonSerializationService = jsonSerializationService;
        }

        public async Task<JobProcessingModel<JobProcessingDetailLookupModel>> GetJobProcessingDetailsForLastHour()
        {
            return _jsonSerializationService.Deserialize<JobProcessingModel<JobProcessingDetailLookupModel>>(await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriodLastHour((short)JobStatusType.Completed, CancellationToken.None));
        }

        public async Task<JobProcessingModel<JobProcessingDetailLookupModel>> GetJobProcessingDetailsForLastFiveMins()
        {
            return _jsonSerializationService.Deserialize<JobProcessingModel<JobProcessingDetailLookupModel>>(await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriodLast5Mins((short)JobStatusType.Completed, CancellationToken.None));
        }

        public async Task<JobProcessingModel<JobProcessingDetailLookupModel>> GetJobProcessingDetailsForCurrentPeriod()
        {
            return _jsonSerializationService.Deserialize<JobProcessingModel<JobProcessingDetailLookupModel>>(await _jobProcessingDetailService.GetJobsProcessingDetailsForCurrentPeriod((short)JobStatusType.Completed, CancellationToken.None));
        }
    }
}
