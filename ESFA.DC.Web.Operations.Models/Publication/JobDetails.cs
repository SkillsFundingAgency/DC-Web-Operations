using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Publication
{
    public class JobDetails
    {
        public long JobId { get; set; }

        public int CollectionYear { get; set; }

        public string StorageReference { get; set; }

        public int PeriodNumber { get; set; }

        public DateTime DateTimeSubmitted { get; set; }

        public string CollectionName { get; set; }

        public string CollectionPrefix { get; set; }
    }
}
