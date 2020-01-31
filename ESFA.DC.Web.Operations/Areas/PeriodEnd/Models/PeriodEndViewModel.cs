using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models.Dtos;

namespace ESFA.DC.Web.Operations.Areas.PeriodEnd.Models
{
    public class PeriodEndViewModel
    {
        public IEnumerable<PathPathItemsModel> Paths { get; set; }

        public bool IsCurrentPeriod { get; set; }

        public int Period { get; set; }

        public int Year { get; set; }

        public bool PeriodEndStarted { get; set; }

        public bool McaReportsPublished { get; set; }

        public bool McaReportsReady { get; set; }

        public bool ProviderReportsPublished { get; set; }

        public bool ProviderReportsReady { get; set; }

        public bool PeriodEndFinished { get; set; }

        public bool ReferenceDataJobsPaused { get; set; }

        public bool CollectionClosed { get; set; }
    }
}