namespace ESFA.DC.Web.Operations.Interfaces.Frm
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using ESFA.DC.PeriodEnd.Models.Dtos;

    public interface IFrmService
    {
        Task<long> RunValidation(string containerName, string folderKey, int periodNumber, string storageReference, string userName, CancellationToken cancellationToken = default(CancellationToken));

        Task<long> RunPublish(long jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> GetFrmStatus(long? jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task<DateTime?> GetFileSubmittedDate(long? jobId, CancellationToken cancellationToken = default(CancellationToken));

        Task PublishSld(int collectionYear, int periodNumber, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<PeriodEndCalendarYearAndPeriodModel>> GetFrmReportsData();

        Task UnpublishSld(string path, CancellationToken cancellationToken = default(CancellationToken));
    }
}
