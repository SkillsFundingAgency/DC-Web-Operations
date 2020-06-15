﻿using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models.ALLF;
using Microsoft.Azure.Storage.Blob;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface ICloudStorageService
    {
        CloudBlobContainer GetStorageContainer();

        CloudBlobContainer GetReferenceDataStorageContainer();

        Task<SubmissionSummary> GetSubmissionSummary(CloudBlobContainer container, string fileName, CancellationToken cancellationToken);
    }
}