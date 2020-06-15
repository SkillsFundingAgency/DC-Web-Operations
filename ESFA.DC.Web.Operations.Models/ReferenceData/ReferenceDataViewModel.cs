using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Models.ReferenceData
{
    public class ReferenceDataViewModel
    {
        public IEnumerable<FileUploadJobMetaDataModel> FileUploads { get; set; }
    }
}