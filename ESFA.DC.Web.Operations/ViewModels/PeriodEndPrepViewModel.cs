using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.ViewModels
{
    public class PeriodEndPrepViewModel
    {
        public int Period { get; set; }

        public int Year { get; set; }

        public string ReferenceDataJobs { get; set; }

        public string FailedJobs { get; set; }

        public bool Closed { get; set; }
    }
}