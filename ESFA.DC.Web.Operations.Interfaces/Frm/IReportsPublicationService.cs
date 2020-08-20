namespace ESFA.DC.Web.Operations.Interfaces.Frm
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.PeriodEnd.Models.Dtos;

    public interface IReportsPublicationService
    {
        Task<long> RunValidationAsync(string collectionName, string folderKey, int periodNumber, string userName, CancellationToken cancellationToken = default(CancellationToken));

        Task<long> RunPublishAsync(long jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> GetFrmStatusAsync(long? jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<Models.Publication.JobDetails> GetFileSubmittedDetailsAsync(long? jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishSldAsync(int collectionYear, int periodNumber, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<PeriodEndCalendarYearAndPeriodModel>> GetFrmReportsDataAsync();

        Task UnpublishSldAsync(int periodNumber, int yearPeriod, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<int>> GetLastTwoCollectionYearsAsync(string collectionType);

        Task UnpublishSldDeleteFolderAsync(string containerName, int period, CancellationToken cancellationToken = default(CancellationToken));
    }
}
