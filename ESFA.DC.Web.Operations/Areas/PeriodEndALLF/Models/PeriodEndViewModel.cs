using System.Collections.Generic;
using ESFA.DC.PeriodEnd.Models.Dtos;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models
{
    public class PeriodEndViewModel
    {
        public int Period { get; set; }

        public int Year { get; set; }

        public bool PeriodEndStarted { get; set; }

        public bool IsCurrentPeriod { get; set; }

        public bool CollectionClosed { get; set; }

        public bool PeriodEndFinished { get; set; }

        public bool ClosePeriodEndEnabled { get; set; }

        public IEnumerable<FileUploadJobMetaDataModel> Files { get; set; }

        public IEnumerable<PathPathItemsModel> Paths { get; set; }
    }
}