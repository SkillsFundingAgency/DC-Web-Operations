using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IALLFPeriodEndService
    {
        Task<IEnumerable<FileUploadJobMetaDataModel>> GetSubmittedFilesPerPeriodAsync(int collectionYear, int period, CancellationToken cancellationToken);
    }
}