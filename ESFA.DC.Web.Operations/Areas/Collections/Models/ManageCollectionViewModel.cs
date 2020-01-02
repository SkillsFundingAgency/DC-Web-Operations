using System;
using System.Collections.Generic;
using ESFA.DC.Web.Operations.Models.Collection;

namespace ESFA.DC.Web.Operations.Areas.Collections.Models
{
    public class ManageCollectionViewModel
    {
        public ManageCollectionViewModel()
        {
            ReturnPeriods = new List<ReturnPeriod>();
        }

        public ManageCollectionViewModel(int collectionId, string collectionName, string currentPeriod, DateTime closingDate, int daysRemaining, string processingOverride)
        {
            CollectionId = collectionId;
            CollectionName = collectionName;
            CurrentPeriod = currentPeriod;
            ClosingDate = closingDate;
            DaysRemaining = daysRemaining;
            ProcessingOverride = processingOverride;
        }

        public int CollectionId { get; set; }

        public string CollectionName { get; set; }

        public string CurrentPeriod { get; set; }

        public DateTime? ClosingDate { get; set; }

        public int? DaysRemaining { get; set; }

        public string ProcessingOverride { get; set; }

        public List<ReturnPeriod> ReturnPeriods { get; set; }
    }
}
