using System;

namespace ESFA.DC.Web.Operations.Models.Collection
{
    public class Collection
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}