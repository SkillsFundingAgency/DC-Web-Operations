using Autofac;
using ESFA.DC.FileService.Config;
using ESFA.DC.FileService.Config.Interface;
using ESFA.DC.Web.Operations.Extensions;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.Extensions.Configuration;

namespace ESFA.DC.Web.Operations.Ioc
{
    public static class ConfigurationRegistration
    {
        public static void SetupConfigurations(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(c =>
                    configuration.GetConfigSection<ConnectionStrings>())
                .As<ConnectionStrings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<AuthenticationSettings>())
                .As<AuthenticationSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<JobQueueApiSettings>())
                .As<JobQueueApiSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<ApiSettings>())
                .As<ApiSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<OpsDataLoadServiceConfigSettings>())
                .As<OpsDataLoadServiceConfigSettings>().SingleInstance();

            builder.Register(c =>
                    configuration.GetConfigSection<AzureStorageSection>())
                .As<AzureStorageSection>().SingleInstance();

            builder.Register(c =>
                configuration.GetConfigSection<ServiceBusSettings>())
                .As<ServiceBusSettings>().SingleInstance();
        }
    }
}