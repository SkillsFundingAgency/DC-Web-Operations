using System;

namespace ESFA.DC.Web.Operations.Services.Event
{
    public sealed class SignalrEventArgs : EventArgs
    {
        public SignalrEventArgs(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; }
    }
}
