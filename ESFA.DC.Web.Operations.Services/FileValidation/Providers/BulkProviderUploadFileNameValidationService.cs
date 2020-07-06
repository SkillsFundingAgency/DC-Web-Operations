﻿using System;
using System.Collections.Generic;
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

namespace ESFA.DC.Web.Operations.Services.FileValidation.Providers
{
    public class BulkProviderUploadFileNameValidationService : AbstractFileNameValidationService
    {
        public BulkProviderUploadFileNameValidationService(
            IStorageService storageService,
            FeatureFlags featureFlags,
            IJobService jobService,
            ICollectionsService collectionService,
            AzureStorageSection azureStorageSection)
            : base(storageService, featureFlags, jobService, collectionService, azureStorageSection)
        {
        }

        protected override IEnumerable<string> FileNameExtensions => new List<string>() { ".CSV" };

        public override async Task<FileNameValidationResultModel> ValidateFileNameAsync(string collectionName, string fileName, long? fileSize, CancellationToken cancellationToken)
        {
            var result = ValidateEmptyFile(fileName, fileSize);
            if (result != null)
            {
                return result;
            }

            string ext = Path.GetExtension(fileName);
            result = ValidateExtension(ext, "Your file must be in a CSV format");
            if (result != null)
            {
                return result;
            }

            var fileNameRegex = await GetFileNameRegexAsync(collectionName, cancellationToken);

            result = ValidateRegex(fileNameRegex, fileName, $"File name should use the format PROVIDERS-yyyymmdd-hhmmss.csv");
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
                System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}