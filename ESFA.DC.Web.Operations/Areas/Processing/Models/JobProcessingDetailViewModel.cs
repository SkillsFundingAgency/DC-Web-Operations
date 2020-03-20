using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model.Processing.Detail;

namespace ESFA.DC.Web.Operations.Areas.Processing.Models
{
    public class JobProcessingDetailViewModel
    {
        public string JobProcessingType { get; set; }

        public List<JobDetails> Data { get; set; }
    }
}
