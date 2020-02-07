using System;

namespace ESFA.DC.Web.Operations.Models.Collection
{
    public class CollectionSummary
    {
        public int CollectionId { get; set; }

        public int CollectionYear { get; set; }

        public string CollectionName { get; set; }

        public bool IsCollectionOpen { get; set; }

        public DateTime? LastPeriodClosedDate { get; set; }

        public int? LastPeriodNumber { get; set; }

        public int? NextPeriodNumber { get; set; }

        public DateTime? NextPeriodOpenDate { get; set; }

        public int? OpenPeriodNumber { get; set; }

        public DateTime? OpenPeriodCloseDate { get; set; }

        public string ManageCollectionsPeriodDisplayValue
        {
            get
            {
                if (OpenPeriodNumber.HasValue)
                {
                    return $"Current period R{OpenPeriodNumber.Value:00}";
                }
                else if (LastPeriodNumber.HasValue)
                {
                    return $"Last period R{LastPeriodNumber.Value:00}";
                } else if (NextPeriodNumber.HasValue)
                {
                    return $"Next period R{NextPeriodNumber.Value:00}";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string ManageCollectionsPeriodDateDisplayValue
        {
            get
            {
                if (IsCurrentPeriod)
                {
                    return $"Closing on {OpenPeriodCloseDate.Value:dd MMMM}";
                }
                else if (IsLastPeriod)
                {
                    return $"Closed on {LastPeriodClosedDate.Value:dd MMMM}";
                }
                else if (IsNextPeriod)
                {
                    return $"Opening on {NextPeriodOpenDate.Value:dd MMMM}";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private bool IsCurrentPeriod => OpenPeriodNumber.HasValue || OpenPeriodCloseDate.HasValue;

        private bool IsLastPeriod => LastPeriodNumber.HasValue || LastPeriodClosedDate.HasValue;

        private bool IsNextPeriod => NextPeriodNumber.HasValue || NextPeriodOpenDate.HasValue;
    }
}
