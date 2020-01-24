﻿using ESFA.DC.Web.Operations.Constants.Authorization;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ESFA.DC.Web.Operations.StartupConfiguration
{
    public static class AuthorisationStartup
    {
        public static void AddAndConfigureAuthorisation(this IServiceCollection services, AuthorizationSettings authSettings)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorisationPolicy.OpsPolicy, policy =>
                    policy.RequireClaim(
                        "http://schemas.portal.com/service",
                        authSettings.OpsClaim));

                options.AddPolicy(AuthorisationPolicy.DevOpsPolicy, policy =>
                    policy.RequireClaim(
                        "http://schemas.portal.com/service",
                        authSettings.OpsClaim,
                        authSettings.DevOpsClaim));
            });
        }
    }
}
