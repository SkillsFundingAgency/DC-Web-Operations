using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using DC.Web.Authorization.Data.Repository;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Web.Operations.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Collections;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Interfaces.Provider;
using ESFA.DC.Web.Operations.Interfaces.Storage;
using ESFA.DC.Web.Operations.Services;
using ESFA.DC.Web.Operations.Services.Collections;
using ESFA.DC.Web.Operations.Services.Hubs;
using ESFA.DC.Web.Operations.Services.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Provider;
using ESFA.DC.Web.Operations.Services.Storage;
using ESFA.DC.Web.Operations.Settings.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ESFA.DC.Web.Operations.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureStorageFileService>().As<IFileService>();

            builder.RegisterType<AuthorizeRepository>().As<IAuthorizeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndService>().As<IPeriodEndService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailDistributionService>().As<IEmailDistributionService>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodService>().As<IPeriodService>().InstancePerLifetimeScope();
            builder.RegisterType<StorageService>().As<IStorageService>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<HubEventBase>().As<IHubEventBase>().SingleInstance();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<HttpClient>().SingleInstance();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<HistoryService>().As<IHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<StateService>().As<IStateService>().InstancePerLifetimeScope();

            builder.RegisterType<PeriodEndStateFactory>().As<IPeriodEndStateFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AddNewProviderService>().As<IAddNewProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<SearchProviderService>().As<ISearchProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<ManageProvidersService>().As<IManageProvidersService>().InstancePerLifetimeScope();
            builder.RegisterType<ManageAssignmentsService>().As<IManageAssignmentsService>().InstancePerLifetimeScope();
            builder.RegisterType<CollectionsService>().As<ICollectionsService>().InstancePerLifetimeScope();
            builder.RegisterType<JobService>().As<IJobService>().InstancePerLifetimeScope();
            builder.RegisterType<FileNameValidationService>().As<IFileNameValidationService>().InstancePerLifetimeScope();

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().InstancePerLifetimeScope();

            // DB Contexts
            builder.RegisterType<Topics.Data.JobQueueDataContext>().SingleInstance();

            builder.Register(context =>
                {
                    var config = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<Topics.Data.JobQueueDataContext>();
                    optionsBuilder.UseSqlServer(
                        config.JobManagement,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<Topics.Data.JobQueueDataContext>>()
                .SingleInstance();
        }
    }
}