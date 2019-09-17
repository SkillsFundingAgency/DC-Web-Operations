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

        Task<bool> SaveEmailTemplate(EmailTemplate template, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<EmailTemplate>> GetEmailTemplates(CancellationToken cancellationToken = default(CancellationToken));

        Task<EmailTemplate> GetEmailTemplate(int emailId, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<Recipient>> GetGroupRecipients(int recipientGroupId, CancellationToken cancellationToken = default);

        Task<bool> RemoveRecipient(int recipientId, int recipientGroupId, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> RemoveGroup(int recipientGroupId, CancellationToken cancellationToken = default(CancellationToken));

        Task<RecipientGroup> GetGroup(int recipientGroupId, CancellationToken cancellationToken = default);
    }
}