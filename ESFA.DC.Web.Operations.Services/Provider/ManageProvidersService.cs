using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class ManageProvidersService : ProviderBaseService, IManageProvidersService
    {
        public ManageProvidersService(
            ApiSettings apiSettings,
            IDateTimeProvider dateTimeProvider,
            IHttpClientService httpClientService)
            : base(apiSettings, dateTimeProvider, httpClientService)
        {
        }
    }
}