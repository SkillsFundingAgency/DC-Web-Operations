using System;

namespace ESFA.DC.Web.Operations.Models
{
    public class FileUploadJobMetaDataModel
    {
        public long JobId { get; set; }

        public int JobStatus { get; set; }

        public string FileName { get; set; }

        public int RecordCount { get; set; }

        public int ErrorCount { get; set; }

        public string ReportName { get; set; }
    }
}