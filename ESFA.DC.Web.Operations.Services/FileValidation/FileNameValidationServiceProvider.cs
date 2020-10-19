using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Services.FileValidation.StandardValidator;

namespace ESFA.DC.Web.Operations.Services.FileValidation
{
    public class FileNameValidationServiceProvider : IFileNameValidationServiceProvider
    {
        private readonly IEnumerable<IFileNameValidationService> _fileNameValidationServices;

        public FileNameValidationServiceProvider(
            IEnumerable<IFileNameValidationService> fileNameValidationServices)
        {
            _fileNameValidationServices = fileNameValidationServices;
        }

        public IFileNameValidationService GetFileNameValidationService(string collectionName)
        {
            return _fileNameValidationServices.FirstOrDefault(x => x.CollectionName.Equals(collectionName, StringComparison.CurrentCultureIgnoreCase)) ??
                _fileNameValidationServices.FirstOrDefault(x => x.GetType() == typeof(StandardFileNameValidationService));
        }
    }
}
