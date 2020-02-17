using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Settings;
using Newtonsoft.Json;

namespace ESFA.DC.Web.Operations.Settings.Models
{
    public class AzureStorageSection : ISettings
    {
        [JsonRequired]
        public string ConnectionString { get; set; }
    }
}
