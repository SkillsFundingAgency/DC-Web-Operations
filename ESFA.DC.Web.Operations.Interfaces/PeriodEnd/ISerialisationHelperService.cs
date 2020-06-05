namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface ISerialisationHelperService
    {
        string SerialiseToCamelCase<T>(T model);
    }
}