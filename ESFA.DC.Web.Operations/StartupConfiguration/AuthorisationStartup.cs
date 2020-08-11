using ESFA.DC.Web.Operations.Security.Policies;
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
                options.AddPolicy(AuthorisationPolicy.DevOpsPolicy, policy =>
                    policy.RequireClaim(
                        "http://schemas.portal.com/service",
                        authSettings.DevOpsClaim));

                options.AddPolicy(AuthorisationPolicy.AdvancedSupportPolicy, policy =>
                    policy.RequireClaim(
                        "http://schemas.portal.com/service",
                        authSettings.AdvancedSupportClaim));

                options.AddPolicy(AuthorisationPolicy.ReportsPolicy, policy =>
                    policy.RequireClaim(
                        "http://schemas.portal.com/service",
                        authSettings.ReportsClaim,
                        authSettings.DevOpsClaim,
                        authSettings.AdvancedSupportClaim));

                options.AddPolicy(AuthorisationPolicy.AdvancedSupportOrDevOpsPolicy, policy =>
                    policy.RequireClaim(
                        "http://schemas.portal.com/service",
                        authSettings.DevOpsClaim,
                        authSettings.AdvancedSupportClaim));

                options.AddPolicy(AuthorisationPolicy.AdvancedSupportOrDevOpsOrReportsPolicy, policy =>
                    policy.RequireClaim(
                        "http://schemas.portal.com/service",
                        authSettings.DevOpsClaim,
                        authSettings.AdvancedSupportClaim,
                        authSettings.ReportsClaim));
            });
        }
    }
}
