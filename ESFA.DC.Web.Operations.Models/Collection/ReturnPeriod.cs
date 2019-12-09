using System;

namespace ESFA.DC.Web.Operations.Models.Collection
{
    public class ReturnPeriod
    {
        public ReturnPeriod()
        {
        }

        public ReturnPeriod(int returnPeriodId, string name, DateTime openDate, DateTime closeDate, bool isLatestOrFuture = false)
        {
            ReturnPeriodId = returnPeriodId;
            Name = name;
            OpenDate = openDate;
            CloseDate = closeDate;
            IsLatestOrFuture = isLatestOrFuture;
        }

        public int ReturnPeriodId { get; set; }

        public string Name { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime CloseDate { get; set; }

        public bool IsLatestOrFuture { get; set; }

        public string CollectionName { get; set; }

        public int CollectionId { get; set; }
    }
}
