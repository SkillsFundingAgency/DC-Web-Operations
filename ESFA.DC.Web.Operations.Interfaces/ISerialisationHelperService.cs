namespace ESFA.DC.Web.Operations.Interfaces
{
    public interface ISerialisationHelperService
    {
        string SerialiseToCamelCase<T>(T model);
    }
}