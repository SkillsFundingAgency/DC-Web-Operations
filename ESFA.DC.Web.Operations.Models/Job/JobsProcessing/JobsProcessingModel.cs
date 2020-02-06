using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Models.JobsProcessing
{
    public class JobsProcessingModel
    {
        public List<JobProcessingLookupModel> Jobs { get; set; }

        public int JobCount { get; set; }
    }
}
