using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.ViewModels
{
    public class PeriodEndPrepViewModel
    {
        public PeriodEndPrepViewModel()
        {
            ReferenceDataJobs = new List<ReferenceDataJobViewModel>();
            FailedJobs = new List<FailedJob>();
        }

        public int Period { get; set; }

        public int Year { get; set; }

        public IEnumerable<ReferenceDataJobViewModel> ReferenceDataJobs { get; set; }

        public IEnumerable<FailedJob> FailedJobs { get; set; }
    }
}