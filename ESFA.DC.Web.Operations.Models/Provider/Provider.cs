namespace ESFA.DC.Web.Operations.Models.Provider
{
    public class Provider
    {
        public Provider(string name, long ukprn, string upin, bool? isMca)
        {
            Name = name;
            Ukprn = ukprn;
            Upin = upin;
            IsMca = isMca;
        }

        public string Name { get; set; }
        public long Ukprn { get; set; }
        public string Upin { get; set; }
        public bool? IsMca { get; set; }
    }
}
