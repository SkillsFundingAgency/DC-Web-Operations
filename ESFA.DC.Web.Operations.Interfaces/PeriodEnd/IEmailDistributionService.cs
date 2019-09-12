using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.PeriodEnd.Models.EmailDistribution;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IEmailDistributionService
    {
        Task<List<RecipientGroup>> GetEmailRecipientGroups(
            CancellationToken cancellationToken = default(CancellationToken));
    }
}