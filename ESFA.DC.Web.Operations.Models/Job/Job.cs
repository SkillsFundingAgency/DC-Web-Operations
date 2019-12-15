using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Job
{
    public class Job
    {
        public string CollectionName { get; set; }

        public int Period { get; set; }

        public string FileName { get; set; }

        public decimal FileSizeBytes { get; set; }

        public string SubmittedBy { get; set; }

        public long Ukprn { get; set; }

        public string NotifyEmail { get; set; }

        public string StorageReference { get; set; }

        public int CollectionYear { get; set; }

        public bool? TermsAccepted { get; set; }
    }
}
