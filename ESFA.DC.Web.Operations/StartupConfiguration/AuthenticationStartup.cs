using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ESFA.DC.Web.Operations.StartupConfiguration
{
    public static class AuthenticationStartup
    {
        public static void AddAndConfigureAuthentication(this IServiceCollection services, AuthenticationSettings authSettings)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.Domain = authSettings.Domain;
                    options.AccessDeniedPath = new PathString("/NotAuthorized");
                })
                .AddWsFederation(options =>
                {
                    options.MetadataAddress = authSettings.MetadataAddress;
                    options.RequireHttpsMetadata = false;
                    options.Wtrealm = authSettings.WtRealm;
                    options.CallbackPath = "/";
                    options.SkipUnrecognizedRequests = true;
                });
        }
    }
}