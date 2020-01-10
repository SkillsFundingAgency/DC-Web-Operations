namespace ESFA.DC.Web.Operations.Areas.Provider.Models
{
    public class DownloadResultsViewModel
    {
        public long TotalSuccessful { get; set; }

        public long TotalFailed { get; set; }

        public bool ErrorFileExists { get; set; }

        public long JobId { get; set; }

        public string CollectionName { get; set; }

        public string ErrorsFileName { get; set; }

        public string ContainerName { get; set; }
    }
}
