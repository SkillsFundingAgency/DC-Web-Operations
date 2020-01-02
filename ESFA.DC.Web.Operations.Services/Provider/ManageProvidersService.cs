namespace ESFA.DC.Web.Operations.Services.Provider
{
    using System.Net.Http;
    using ESFA.DC.Logging.Interfaces;
    using ESFA.DC.Serialization.Interfaces;
    using ESFA.DC.Web.Operations.Interfaces.Provider;
    using ESFA.DC.Web.Operations.Settings.Models;

    public class ManageProvidersService : ProviderBaseService, IManageProvidersService
    {
        public ManageProvidersService(
            ILogger logger,
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient)
            : base(logger, jsonSerializationService, apiSettings, httpClient)
        {
        }
    }
}
