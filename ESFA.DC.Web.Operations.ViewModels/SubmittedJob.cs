using System;

namespace ESFA.DC.Web.Operations.ViewModels
{
    public class FailedJob
    {
        public long JobId { get; set; }

        public DateTime? DateTimeSubmitted { get; set; }

        public long Ukprn { get; set; }

        public string OrganisationName { get; set; }

        public string CollectionName { get; set; }

        public string FileName { get; set; }
    }
}