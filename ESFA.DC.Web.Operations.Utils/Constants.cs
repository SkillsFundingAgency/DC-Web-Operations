namespace ESFA.DC.Web.Operations.Utils
{
    public class Constants
    {
        public const string IlrCollectionNamePrefix = "ILR";
        public const int YearStartMonth = 8;

        public const string CollectionYearToken = "{collectionYear}";
        public static readonly string PeriodEndBlobContainerName = "periodend" + CollectionYearToken + "-files";
    }
}
