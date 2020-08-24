using System;
using System.Globalization;
using System.IO;
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

namespace ESFA.DC.Web.Operations.Services.FileValidation.CampusIdentifiers
{
    public class DevolvedContractsFileNameValidationService : AbstractFileNameValidationService
    {
        public DevolvedContractsFileNameValidationService(
            IStorageService storageService,
            FeatureFlags featureFlags,
            IJobService jobService,
            ICollectionsService collectionService,
            AzureStorageSection azureStorageSection)
            : base(storageService, featureFlags, jobService, collectionService, azureStorageSection)
        {
        }

        public override string CollectionName => CollectionNames.DevolvedContracts;

        protected override string FileNameFormat => "DevolvedContractsRD-YYYYMMDDHHMM.csv";

        public override async Task<FileNameValidationResultModel> ValidateFileNameAsync(string fileName, long? fileSize, CancellationToken cancellationToken)
        {
            var result = ValidateEmptyFile(fileName, fileSize);
            if (result != null)
            {
                return result;
            }

            var ext = Path.GetExtension(fileName);
            result = ValidateExtension(ext, string.Format(CultureInfo.CurrentCulture, FileNameValidationConsts.FileMustBeInFormat, string.Join(",", FileNameExtensions)));
            if (result != null)
            {
                return result;
            }

            var fileNameRegex = await GetFileNameRegexAsync(CollectionName, cancellationToken);

            result = ValidateRegex(fileNameRegex, fileName, string.Format(CultureInfo.CurrentCulture, FileNameValidationConsts.FileNameMustBeInFormat, FileNameFormat));
            if (result != null)
            {
                return result;
            }

            result = await ValidateUniqueFileAsync(CollectionName, fileName, cancellationToken);
            if (result != null)
            {
                return result;
            }

            result = await LaterFileExistsAsync(CollectionName, fileNameRegex, fileName, cancellationToken);
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
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("fileName is null");
            }

            return DateTime.ParseExact(
                $"{Path.GetFileNameWithoutExtension(fileName).Split('-')[1]}",
                "yyyyMMddHHmm",
                CultureInfo.InvariantCulture);
        }
    }
}