using System;
using System.Collections.Generic;
using System.Text;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.ReferenceData;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Services.FileValidation.CampusIdentifiers;
using ESFA.DC.Web.Operations.Services.FileValidation.ConditionOfFundingRemoval;
using ESFA.DC.Web.Operations.Services.FileValidation.Providers;
using ESFA.DC.Web.Operations.Services.FileValidation.ValidationMessages2021;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.FileValidation
{
    public class FileNameValidationServiceProvider : IFileNameValidationServiceProvider
    {
        private readonly IStorageService _storageService;
        private readonly FeatureFlags _featureFlags;
        private readonly IJobService _jobService;
        private readonly ICollectionsService _collectionsService;
        private readonly AzureStorageSection _azureStorageSection;

        public FileNameValidationServiceProvider(
            IStorageService storageService,
            FeatureFlags featureFlags,
            IJobService jobService,
            ICollectionsService collectionService,
            AzureStorageSection azureStorageSection)
        {
            _storageService = storageService;
            _featureFlags = featureFlags;
            _jobService = jobService;
            _collectionsService = collectionService;
            _azureStorageSection = azureStorageSection;
        }

        public IFileNameValidationService GetFileNameValidationService(string collectionName)
        {
            switch (collectionName)
            {
                case CollectionNames.ReferenceDataCampusIdentifiers:
                    return new CampusIdentifiersFileNameValidationService(_storageService, _featureFlags, _jobService, _collectionsService, _azureStorageSection);
                case CollectionNames.ReferenceDataConditionsOfFundingRemoval:
                    return new ConditionOfFundingRemovalFileNameValidationService(_storageService, _featureFlags, _jobService, _collectionsService, _azureStorageSection);
                case CollectionNames.ReferenceDataValidationMessages2021:
                    return new ValidationMessages2021FileNameValidationService(_storageService, _featureFlags, _jobService, _collectionsService, _azureStorageSection);
                case CollectionNames.ReferenceDataOps:
                    return new BulkProviderUploadFileNameValidationService(_storageService, _featureFlags, _jobService, _collectionsService, _azureStorageSection);
                default:
                    throw new ArgumentException("Invalid collectionName");
            }
        }
    }
}
