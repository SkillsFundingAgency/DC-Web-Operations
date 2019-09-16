using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.EmailDistribution.Models;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IEmailDistributionService
    {
        Task<List<RecipientGroup>> GetEmailRecipientGroups(
            CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> SaveGroup(string groupName, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> SaveRecipient(Recipient recipient, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> IsDuplicateGroupName(string groupName,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}