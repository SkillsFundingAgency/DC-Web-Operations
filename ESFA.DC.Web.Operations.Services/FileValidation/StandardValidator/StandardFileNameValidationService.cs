using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.FileValidation.StandardValidator
{
    public class StandardFileNameValidationService : AbstractFileNameValidationService
    {
        public StandardFileNameValidationService(
            IStorageService storageService,
            FeatureFlags featureFlags,
            IJobService jobService,
            ICollectionsService collectionService,
            AzureStorageSection azureStorageSection,
            IEnumerable<ICollection> collections)
            : base(storageService, featureFlags, jobService, collectionService, azureStorageSection, collections)
        {
        }

        public override string CollectionName => string.Empty;

        public override DateTime GetFileDateTime(Regex fileNameRegex, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("fileName is null");
            }

            return DateTime.ParseExact(
                $"{Path.GetFileNameWithoutExtension(fileName).Split('-').Last()}",
                "yyyyMMddHHmm",
                CultureInfo.InvariantCulture);
        }
    }
}