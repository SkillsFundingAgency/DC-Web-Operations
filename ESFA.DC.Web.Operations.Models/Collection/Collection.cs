using System;

namespace ESFA.DC.Web.Operations.Models.Collection
{
    public class Collection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool? ProcessingOverride { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ContainerName { get; set; }

        public bool IsOpen { get; set; }

        public int CollectionYear { get; set; }

        public string FileNameRegex { get; set; }
    }
}