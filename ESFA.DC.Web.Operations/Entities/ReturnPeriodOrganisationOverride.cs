namespace ESFA.DC.Web.Operations.Entities
{
    public partial class ReturnPeriodOrganisationOverride
    {
        public int Id { get; set; }

        public int ReturnPeriodId { get; set; }

        public int OrgaisationId { get; set; }

        public int PeriodNumber { get; set; }

        public virtual Organisation Orgaisation { get; set; }

        public virtual ReturnPeriod ReturnPeriod { get; set; }
    }
}
