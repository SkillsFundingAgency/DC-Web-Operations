using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.ViewModels
{
    public class PeriodEndViewModel
    {
        public PeriodEndViewModel()
        {
            ReferenceDataJobs = new List<ReferenceDataJobViewModel>();
        }

        public string Paths { get; set; }

        public int CurrentPeriod { get; set; }

        public int Period { get; set; }

        public int Year { get; set; }

        public bool ReportsPublished { get; set; }

        public bool Closed { get; set; }

        public IEnumerable<ReferenceDataJobViewModel> ReferenceDataJobs { get; set; }
    }
}