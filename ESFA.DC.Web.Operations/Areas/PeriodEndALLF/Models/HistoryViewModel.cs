using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Areas.PeriodEndALLF.Models
{
    public class HistoryViewModel
    {
        public int Year { get; set; }

        public IEnumerable<int> CollectionYears { get; set; }

        public IEnumerable<FileUploadJobMetaDataModel> PeriodHistories { get; set; }
    }
}