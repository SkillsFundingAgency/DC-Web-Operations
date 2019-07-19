namespace ESFA.DC.Web.Operations.Models.PeriodEnd
{
    public class PathYearPeriod
    {
        public int PathId { get; set; }

        public int Year { get; set; }

        public int Period { get; set; }

        public bool ReportsPublished { get; set; }

        public bool PeriodClosed { get; set; }
    }
}