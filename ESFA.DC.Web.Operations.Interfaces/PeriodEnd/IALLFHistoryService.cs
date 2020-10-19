using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.Web.Operations.Models;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IALLFHistoryService
    {
        Task<IEnumerable<FileUploadJobMetaDataModel>> GetHistoryDetails(int currentYear, int currentPeriod, CancellationToken cancellationToken);
    }
}