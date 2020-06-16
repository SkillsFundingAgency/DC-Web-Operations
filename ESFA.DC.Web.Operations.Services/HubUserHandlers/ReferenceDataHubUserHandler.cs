using System.Collections.Generic;
using ESFA.DC.Web.Operations.Utils;
using Microsoft.EntityFrameworkCore.Internal;

namespace ESFA.DC.Web.Operations.Services.HubUserHandlers
{
    public static class ReferenceDataHubUserHandler
    {
        private static readonly Dictionary<ReferenceDataTypes, HashSet<string>> ReferenceDataConnectedIds = new Dictionary<ReferenceDataTypes, HashSet<string>>();
        private static readonly object LockObject = new object();

        public static void AddConnectionId(ReferenceDataTypes type, string connectionId)
        {
            lock (LockObject)
            {
                var found = ReferenceDataConnectedIds.TryGetValue(type, out var connectionIds);

                if (!found)
                {
                    connectionIds = new HashSet<string>();
                    ReferenceDataConnectedIds.Add(ReferenceDataTypes.ConditionOfFundingRemoval, connectionIds);
                }

                connectionIds.Add(connectionId);
            }
        }

        public static void RemoveConnectionId(ReferenceDataTypes type, string connectionId)
        {
            lock (LockObject)
            {
                if (ReferenceDataConnectedIds.ContainsKey(type))
                {
                    ReferenceDataConnectedIds[type].Remove(connectionId);
                }
            }
        }

        public static bool AnyConnectionIds(ReferenceDataTypes type)
        {
            lock (LockObject)
            {
                var found = ReferenceDataConnectedIds.TryGetValue(type, out var connectionIds);

                return found && connectionIds.Any();
            }
        }

        public static void ClearAll()
        {
            lock (LockObject)
            {
                ReferenceDataConnectedIds.Clear();
            }
        }
    }
}