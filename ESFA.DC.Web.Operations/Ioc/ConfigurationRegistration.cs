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
                configuration.GetConfigSection<AzureStorageFileServiceConfiguration>("AzureStorageSection"))
                .As<IAzureStorageFileServiceConfiguration>().SingleInstance();

            //builder.Register(c =>
            //        configuration.GetConfigSection<FeatureFlags>())
            //    .As<FeatureFlags>().SingleInstance();

            //builder.Register(c => configuration.GetConfigSection<CloudStorageSettings>("EsfCloudStorageSettings"))
            //    .Keyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.EsfSubmission).SingleInstance();
            //builder.Register(c => configuration.GetConfigSection<CloudStorageSettings>("EsfR2CloudStorageSettings"))
            //    .Keyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.Esf2Submission).SingleInstance();
            //builder.Register(c => configuration.GetConfigSection<CloudStorageSettings>("IlrCloudStorageSettings"))
            //    .Keyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.IlrSubmission).SingleInstance();
            //builder.Register(c => configuration.GetConfigSection<CloudStorageSettings>("EasCloudStorageSettings"))
            //    .Keyed<IAzureStorageKeyValuePersistenceServiceConfig>(EnumJobType.EasSubmission).SingleInstance();

            //builder.Register(c => configuration.GetConfigSection<CrossLoadingQueueConfiguration>()).As<IQueueConfiguration>().SingleInstance();
        }
    }
}