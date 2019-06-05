using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace ESFA.DC.Web.Operations.Hubs
{
    public class OperationsHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}