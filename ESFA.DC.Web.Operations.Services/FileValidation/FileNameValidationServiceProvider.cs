using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Services.FileValidation.StandardValidator;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.FileValidation
{
    public class FileNameValidationServiceProvider : IFileNameValidationServiceProvider
    {
        private readonly IStorageService _storageService;
        private readonly FeatureFlags _featureFlags;
        private readonly IJobService _jobService;
        private readonly ICollectionsService _collectionService;
        private readonly AzureStorageSection _azureStorageSection;
        private readonly IEnumerable<ICollection> _collections;
        private readonly IEnumerable<IFileNameValidationService> _fileNameValidationServices;

        public FileNameValidationServiceProvider(
            IStorageService storageService,
            FeatureFlags featureFlags,
            IJobService jobService,
            ICollectionsService collectionService,
            AzureStorageSection azureStorageSection,
            IEnumerable<ICollection> collections,
            IEnumerable<IFileNameValidationService> fileNameValidationServices)
        {
            _storageService = storageService;
            _featureFlags = featureFlags;
            _jobService = jobService;
            _collectionService = collectionService;
            _azureStorageSection = azureStorageSection;
            _collections = collections;
            _fileNameValidationServices = fileNameValidationServices;
        }

        public IFileNameValidationService GetFileNameValidationService(string collectionName)
        {
            return _fileNameValidationServices.FirstOrDefault(x => x.CollectionName.Equals(collectionName, StringComparison.CurrentCultureIgnoreCase)) ??
                   new StandardFileNameValidationService(
                    _storageService,
                    _featureFlags,
                    _jobService,
                    _collectionService,
                    _azureStorageSection,
                    _collections);
        }
    }
}
