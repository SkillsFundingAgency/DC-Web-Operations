using System.Collections.Generic;
using ESFA.DC.Jobs.Model;

namespace ESFA.DC.Web.Operations.Areas.Collections.Models
{
    public class ManageCollectionsCollectionOverrideViewModel
    {
        public string CollectionName { get; set; }

        public int CollectionId { get; set; }

        public int ProcessingOverride { get; set; }

        public IList<FileUploadJob> Jobs { get; set; }
    }
}
