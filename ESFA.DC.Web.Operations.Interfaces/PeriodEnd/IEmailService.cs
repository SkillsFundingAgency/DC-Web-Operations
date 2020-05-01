using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.PeriodEnd
{
    public interface IEmailService
    {
        Task SendEmail(int hubEmailId, int periodNumber, string periodPrefix, CancellationToken cancellationToken = default);
    }
}