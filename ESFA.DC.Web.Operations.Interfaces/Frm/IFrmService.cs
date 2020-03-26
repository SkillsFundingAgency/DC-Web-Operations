namespace ESFA.DC.Web.Operations.Interfaces.Frm
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.PeriodEnd.Models.Dtos;

    public interface IFrmService
    {
        Task<long> RunValidationAsync(string containerName, string folderKey, int periodNumber, string storageReference, string userName, CancellationToken cancellationToken = default(CancellationToken));

        Task<long> RunPublishAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> GetFrmStatusAsync(long? jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<DateTime?> GetFileSubmittedDateAsync(long? jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishSldAsync(int collectionYear, int periodNumber, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<PeriodEndCalendarYearAndPeriodModel>> GetFrmReportsDataAsync();

        Task UnpublishSldAsync(string path, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<int>> GetLastTwoCollectionYearsAsync(string collectionType);
    }
}
