﻿using System.Net.Http;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Settings.Models;

namespace ESFA.DC.Web.Operations.Services.Provider
{
    public class ManageProvidersService : ProviderBaseService, IManageProvidersService
    {
        public ManageProvidersService(
            IJsonSerializationService jsonSerializationService,
            ApiSettings apiSettings,
            HttpClient httpClient,
            IDateTimeProvider dateTimeProvider)
            : base(jsonSerializationService, apiSettings, httpClient, dateTimeProvider)
        {
        }
    }
}
