using System;

namespace ESFA.DC.Web.Operations.Models
{
    public class FileUploadJobMetaDataModel
    {
        public long JobId { get; set; }

        public int JobStatus { get; set; }

        public string DisplayStatus { get; set; }

        public int PeriodNumber { get; set; }

        public string FileName { get; set; }

        public int RecordCount { get; set; }

        public int ErrorCount { get; set; }

        public string ReportName { get; set; }

        public bool UsedForPeriodEnd { get; set; }

        public DateTime SubmissionDate { get; set; }

        public string SubmittedBy { get; set; }

        public string DisplayDate { get; set; }

        public string CollectionName { get; set; }
    }
}