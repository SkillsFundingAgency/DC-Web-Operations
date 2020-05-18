using System;

namespace ESFA.DC.Web.Operations.Models.PeriodEnd
{
    public class ALLFHistoryDetail
    {
        public int Period { get; set; }

        public int PeriodEndId { get; set; }

        public int Year { get; set; }

        public DateTime Date { get; set; }

        public string SubmittedBy { get; set; }

        public string File { get; set; }

        public bool PeriodEnd { get; set; }

        public string Return { get; set; }

        public string ReportFile { get; set; }

        public int Status { get; set; }

        public int Records { get; set; }

        public int Warnings { get; set; }
    }
}