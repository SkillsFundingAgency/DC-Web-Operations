namespace ESFA.DC.Web.Operations.Models.Provider
{
    public class Provider
    {
        public Provider(string name, long ukprn, int? upin, bool isMca, bool isEnabled)
        {
            Name = name;
            Ukprn = ukprn;
            Upin = upin;
            IsMca = isMca;
            IsEnabled = isEnabled;
        }

        public string Name { get; set; }
        public long Ukprn { get; set; }
        public int? Upin { get; set; }
        public bool IsMca { get; set; }
        public bool IsEnabled { get; set; }
    }
}
