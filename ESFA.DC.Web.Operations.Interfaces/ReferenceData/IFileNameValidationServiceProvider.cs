using System.Collections.Generic;

namespace ESFA.DC.Web.Operations.Interfaces.ReferenceData
{
    public interface IFileNameValidationServiceProvider
    {
        IFileNameValidationService GetFileNameValidationService(string collectionName);

        IEnumerable<IFileNameValidationService> GetFileNameValidationServices(string[] collectionNames);
    }
}
