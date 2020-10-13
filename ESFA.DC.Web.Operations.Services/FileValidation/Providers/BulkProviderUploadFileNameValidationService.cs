using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Models;
using ESFA.DC.Web.Operations.Models.Enums;
using ESFA.DC.Web.Operations.Settings.Models;
using ESFA.DC.Web.Operations.Utils;

namespace ESFA.DC.Web.Operations.Services.FileValidation.Providers
{
    public class BulkProviderUploadFileNameValidationService : AbstractFileNameValidationService
    {
        private readonly IEnumerable<ICollection> _collections;

        public BulkProviderUploadFileNameValidationService(
            IStorageService storageService,
            FeatureFlags featureFlags,
            IJobService jobService,
            ICollectionsService collectionService,
            AzureStorageSection azureStorageSection,
            IEnumerable<ICollection> collections)
            : base(storageService, featureFlags, jobService, collectionService, azureStorageSection, collections)
        {
            _collections = collections;
        }

        public override string CollectionNames => Utils.CollectionNames.ReferenceDataOps;

        public override async Task<FileNameValidationResultModel> ValidateFileNameAsync(string collectionName, string fileName, string fileNameFormat, long? fileSize, CancellationToken cancellationToken)
        {
            var collection = _collections.SingleOrDefault(s => string.Equals(s.CollectionName, collectionName, StringComparison.CurrentCultureIgnoreCase));

            if (collection == null)
            {
                throw new ArgumentOutOfRangeException(nameof(collectionName));
            }

            var result = ValidateEmptyFile(fileName, fileSize);
            if (result != null)
            {
                return result;
            }

            string ext = Path.GetExtension(fileName);
            result = ValidateExtension(collection.FileFormat, ext, string.Format(CultureInfo.CurrentCulture, FileNameValidationConsts.FileMustBeInFormat, string.Join(",", collection.FileFormat)));
            if (result != null)
            {
                return result;
            }

            var fileNameRegex = await GetFileNameRegexAsync(collection.CollectionName, cancellationToken);

            result = ValidateRegex(fileNameRegex, fileName, string.Format(CultureInfo.CurrentCulture, FileNameValidationConsts.FileNameMustBeInFormat, collection.FileNameFormat));
            if (result != null)
            {
                return result;
            }

            return new FileNameValidationResultModel()
            {
                ValidationResult = FileNameValidationResult.Valid
            };
        }

        public override DateTime GetFileDateTime(Regex fileNameRegex, string fileName)
        {
            var matches = fileNameRegex.Match(fileName);

            return DateTime.ParseExact(
                $"{matches.Groups[4].Value}-{matches.Groups[8].Value}",
                "yyyyMMdd-HHmmss",
                CultureInfo.InvariantCulture);
        }
    }
}