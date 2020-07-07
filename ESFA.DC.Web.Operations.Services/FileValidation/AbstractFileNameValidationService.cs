using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.CollectionsManagement.Models;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.FileValidation
{
    public abstract class AbstractFileNameValidationService : IFileNameValidationService
    {
        private readonly IStorageService _storageService;
        private readonly FeatureFlags _featureFlags;
        private readonly IJobService _jobService;
        private readonly ICollectionsService _collectionService;
        private readonly AzureStorageSection _azureStorageConfig;

        private Dictionary<string, Collection> _collections = new Dictionary<string, Collection>();

        protected AbstractFileNameValidationService(
            IStorageService storageService,
            FeatureFlags featureFlags,
            IJobService jobService,
            ICollectionsService collectionService,
            AzureStorageSection azureStorageConfig)
        {
            _storageService = storageService;
            _featureFlags = featureFlags;
            _jobService = jobService;
            _collectionService = collectionService;
            _azureStorageConfig = azureStorageConfig;
        }

        public abstract string CollectionName { get; }

        protected virtual IEnumerable<string> FileNameExtensions => new List<string>() { ".CSV" };

        protected abstract string FileNameFormat { get; }

        public abstract Task<FileNameValidationResultModel> ValidateFileNameAsync(string fileName, long? fileSize, CancellationToken cancellationToken);

        public abstract DateTime GetFileDateTime(Regex fileNameRegex, string fileName);

        public FileNameValidationResultModel ValidateEmptyFile(string fileName, long? fileSize)
        {
            if (string.IsNullOrEmpty(fileName) || fileSize == null || fileSize.Value == 0)
            {
                return new FileNameValidationResultModel()
                {
                    ValidationResult = FileNameValidationResult.EmptyFile,
                    FieldError = "Choose a file to upload",
                    SummaryError = "Check file you want to upload"
                };
            }

            return null;
        }

        public async Task<Regex> GetFileNameRegexAsync(string collectionName, CancellationToken cancellationToken)
        {
            Collection collection;
            if (!_collections.TryGetValue(collectionName, out collection))
            {
                _collections.Add(collectionName, await _collectionService.GetCollectionAsync(collectionName, cancellationToken));
                collection = _collections[collectionName];
            }

            var fileNameRegex = collection.FileNameRegex;

            return !string.IsNullOrEmpty(fileNameRegex) ? new Regex(fileNameRegex, RegexOptions.Compiled) : null;
        }

        public FileNameValidationResultModel ValidateExtension(string extension, string errorMessage)
        {
            if (!FileNameExtensions.Contains(extension.ToUpperInvariant()))
            {
                return new FileNameValidationResultModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileExtension,
                    FieldError = errorMessage,
                    SummaryError = errorMessage
                };
            }

            return null;
        }

        public FileNameValidationResultModel ValidateRegex(Regex fileNameRegex, string fileName, string errorMessage)
        {
            if (!IsValidRegex(fileNameRegex, fileName))
            {
                return new FileNameValidationResultModel()
                {
                    ValidationResult = FileNameValidationResult.InvalidFileNameFormat,
                    FieldError = errorMessage,
                    SummaryError = errorMessage
                };
            }

            return null;
        }

        public bool IsValidRegex(Regex fileNameRegex, string fileName)
        {
            return fileNameRegex.IsMatch(fileName);
        }

        public async Task<FileNameValidationResultModel> ValidateUniqueFileAsync(string collectionName, string fileName, CancellationToken cancellationToken)
        {
            Collection collection;
            if (!_collections.TryGetValue(collectionName, out collection))
            {
                _collections.Add(collectionName, await _collectionService.GetCollectionAsync(collectionName, cancellationToken));
                collection = _collections[collectionName];
            }

            if (_featureFlags.DuplicateFileCheckEnabled)
            {
                if (await (await _storageService.GetAzureStorageReferenceService(_azureStorageConfig.ConnectionString, collection.StorageReference)).ContainsAsync($"{fileName}", cancellationToken))
                {
                    return new FileNameValidationResultModel()
                    {
                        ValidationResult = FileNameValidationResult.FileAlreadyExists,
                        FieldError = "Filename date is the same as the currently loaded version",
                        SummaryError = "Filename date is the same as the currently loaded version"
                    };
                }
            }

            return null;
        }

        public virtual async Task<FileNameValidationResultModel> LaterFileExistsAsync(string collectionName, Regex fileNameRegex, string fileName, CancellationToken cancellationToken)
        {
            var job = await _jobService.GetLatestJobForCollectionAsync(collectionName, cancellationToken);
            if (job == null)
            {
                return null;
            }

            var fileDateTime = GetFileDateTime(fileNameRegex, fileName);
            var existingJobFileDateTime = GetFileDateTime(fileNameRegex, job.FileName.Split('/')[1]);
            if (fileDateTime < existingJobFileDateTime)
            {
                return new FileNameValidationResultModel()
                {
                    ValidationResult = FileNameValidationResult.LaterFileAlreadySubmitted,
                    FieldError = "Filename date is earlier than the currently loaded version",
                    SummaryError = "Filename date is earlier than the currently loaded version"
                };
            }

            return null;
        }
    }
}
