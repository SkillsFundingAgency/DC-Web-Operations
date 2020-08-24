using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Models.ReferenceData
{
    public class ReferenceDataViewModel
    {
        public string ReferenceDataCollectionName { get; set; }

        public string CollectionDisplayName { get; set; }

        public string HubName { get; set; }

        public string FileExtension { get; set; }

        public IEnumerable<FileUploadJobMetaDataModel> Files { get; set; }
    }
}