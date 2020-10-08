using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.FileValidation
{
    public abstract class AbstractFileNameValidationService : IFileNameValidationService
    {
        private readonly IStorageService _storageService;
        private readonly FeatureFlags _featureFlags;
        private readonly IJobService _jobService;
        private readonly ICollectionsService _collectionService;
        private readonly AzureStorageSection _azureStorageConfig;
        private readonly IEnumerable<ICollection> _collections;

        private Dictionary<string, Collection> _collectionsDb = new Dictionary<string, Collection>();

        protected AbstractFileNameValidationService(
            IStorageService storageService,
            FeatureFlags featureFlags,
            IJobService jobService,
            ICollectionsService collectionService,
            AzureStorageSection azureStorageConfig,
            IEnumerable<ICollection> collections)
        {
            _storageService = storageService;
            _featureFlags = featureFlags;
            _jobService = jobService;
            _collectionService = collectionService;
            _azureStorageConfig = azureStorageConfig;
            _collections = collections;
        }

        public abstract string[] CollectionNames { get; }

        public virtual async Task<FileNameValidationResultModel> ValidateFileNameAsync(string collectionName, string fileName, string fileNameFormat, long? fileSize, CancellationToken cancellationToken)
        {
            var collection = _collections.SingleOrDefault(s => s.CollectionName == collectionName);
            if (collection == null)
            {
                throw new ArgumentOutOfRangeException(nameof(collectionName));
            }

            var result = ValidateEmptyFile(fileName, fileSize);
            if (result != null)
            {
                return result;
            }

            var ext = Path.GetExtension(fileName);
            result = ValidateExtension(collection.FileFormat, ext, string.Format(CultureInfo.CurrentCulture, FileNameValidationConsts.FileMustBeInFormat, string.Join(",", collection.FileFormat)));
            if (result != null)
            {
                return result;
            }

            var fileNameRegex = await GetFileNameRegexAsync(collectionName, cancellationToken);

            result = ValidateRegex(fileNameRegex, fileName, string.Format(CultureInfo.CurrentCulture, FileNameValidationConsts.FileNameMustBeInFormat, fileNameFormat));
            if (result != null)
            {
                return result;
            }

            result = await ValidateUniqueFileAsync(collectionName, fileName, cancellationToken);
            if (result != null)
            {
                return result;
            }

            result = await LaterFileExistsAsync(collectionName, fileNameRegex, fileName, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return new FileNameValidationResultModel()
            {
                ValidationResult = FileNameValidationResult.Valid
            };
        }

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
            if (!_collectionsDb.TryGetValue(collectionName, out collection))
            {
                _collectionsDb.Add(collectionName, await _collectionService.GetCollectionAsync(collectionName, cancellationToken));
                collection = _collectionsDb[collectionName];
            }

            var fileNameRegex = collection.FileNameRegex;

            return !string.IsNullOrEmpty(fileNameRegex) ? new Regex(fileNameRegex, RegexOptions.Compiled) : null;
        }

        public FileNameValidationResultModel ValidateExtension(string fileNameExtension, string extension, string errorMessage)
        {
            if (!string.Equals(fileNameExtension, extension, StringComparison.InvariantCultureIgnoreCase))
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
            if (!_collectionsDb.TryGetValue(collectionName, out collection))
            {
                _collectionsDb.Add(collectionName, await _collectionService.GetCollectionAsync(collectionName, cancellationToken));
                collection = _collectionsDb[collectionName];
            }

            if (_featureFlags.DuplicateFileCheckEnabled)
            {
                if (await (await _storageService.GetAzureStorageReferenceService(_azureStorageConfig.ConnectionString, collection.StorageReference)).ContainsAsync($"{collectionName}/{fileName}", cancellationToken))
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
            var filenameParts = job.FileName.Split('/');
            var existingJobFileDateTime = GetFileDateTime(fileNameRegex, filenameParts.Last());
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
