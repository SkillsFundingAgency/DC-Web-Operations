using System.Globalization;

namespace ESFA.DC.Web.Operations.ViewModels
{
    public class ReturnPeriodViewModel
    {
        private string _periodName;
        private string _fundingClaimPeriodName;

        public ReturnPeriodViewModel(int periodNumber)
        {
            PeriodNumber = periodNumber;
            _periodName = $"R{periodNumber.ToString("00", NumberFormatInfo.InvariantInfo)}";
            SetFundingClaimsPeriod(periodNumber);
        }

        public int PeriodNumber { get; set; }

        public string PeriodName() => _periodName;

        public string FundingClaimPeriodName() => _fundingClaimPeriodName;

        public string NextOpeningDate { get; set; }

        public int DaysToClose { get; set; }

        public string PeriodCloseDate { get; set; }

        private void SetFundingClaimsPeriod(int periodNumber)
        {
            _fundingClaimPeriodName = "year end forecast";
            //if (PeriodNumber == 1)
            //{
            //    _fundingClaimPeriodName = "mid year";
            //}
            //else if (PeriodNumber == 2)
            //{
            //    _fundingClaimPeriodName = "year end";
            //}
            //else if (PeriodNumber == 3)
            //{
            //    _fundingClaimPeriodName = "final";
            //}
        }
    }

    
}
