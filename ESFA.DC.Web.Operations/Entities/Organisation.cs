using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Entities
{
    public partial class Organisation
    {
        public Organisation()
        {
            OrganisationCollection = new HashSet<OrganisationCollection>();
            ReturnPeriodOrganisationOverride = new HashSet<ReturnPeriodOrganisationOverride>();
        }

        public int OrganisationId { get; set; }

        public long Ukprn { get; set; }

        public string Name { get; set; }

        public bool? Enabled { get; set; }

        public bool IsMca { get; set; }

        public virtual ICollection<OrganisationCollection> OrganisationCollection { get; set; }

        public virtual ICollection<ReturnPeriodOrganisationOverride> ReturnPeriodOrganisationOverride { get; set; }
    }
}
