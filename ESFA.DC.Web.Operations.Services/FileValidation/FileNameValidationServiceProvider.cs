using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;

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
            var service = _fileNameValidationServices.FirstOrDefault(x => x.CollectionName == collectionName);
            if (service == null)
            {
                throw new ArgumentException("collectionName invalid");
            }

            return service;
        }

        public IEnumerable<IFileNameValidationService> GetFileNameValidationServices(string[] collectionNames)
        {
            var services = _fileNameValidationServices.Where(x => collectionNames.Contains(x.CollectionName)).ToArray();
            if (!services.Any())
            {
                throw new ArgumentException("collectionNames invalid");
            }

            return services;
        }
    }
}
