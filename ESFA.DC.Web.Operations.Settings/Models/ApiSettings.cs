using ESFA.DC.Web.Operations.Interfaces.Settings;
using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Settings.Models
{
    public class ApiSettings : ISettings
    {
        [JsonRequired]
        public string JobManagementApiBaseUrl { get; set; }
    }
}
