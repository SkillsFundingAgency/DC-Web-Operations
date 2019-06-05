using ESFA.DC.Web.Operations.Interfaces.Settings;
using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Settings.Models
{
    public class ConnectionStrings : ISettings
    {
        [JsonRequired]
        public string AppLogs { get; set; }

        [JsonRequired]
        public string Permissions { get; set; }
    }
}