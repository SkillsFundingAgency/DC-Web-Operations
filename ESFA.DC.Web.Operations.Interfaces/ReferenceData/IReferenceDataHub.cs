using System;
using System.Threading;
using System.Threading.Tasks;

namespace ESFA.DC.Web.Operations.Interfaces.ReferenceData
{
    public interface IReferenceDataHub
    {
        Task OnConnectedAsync();

        Task OnDisconnectedAsync(Exception exception);

        Task SendMessage(CancellationToken cancellationToken);
    }
}