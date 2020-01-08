﻿using DC.Web.Authorization.Data.Repository;

namespace ESFA.DC.Web.Operations.Ioc
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Autofac;
    using ESFA.DC.DateTimeProvider.Interface;
    using ESFA.DC.FileService;
    using ESFA.DC.FileService.Interface;
    using ESFA.DC.JobQueueManager.Data;
    using ESFA.DC.ReferenceData.Organisations.Model;
    using ESFA.DC.ReferenceData.Organisations.Model.Interface;
    using ESFA.DC.Serialization.Interfaces;
    using ESFA.DC.Serialization.Json;
    using ESFA.DC.Web.Operations.Interfaces.Frm;
    using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
    using ESFA.DC.Web.Operations.Interfaces.Provider;
    using ESFA.DC.Web.Operations.Interfaces.Storage;
    using ESFA.DC.Web.Operations.Services;
    using ESFA.DC.Web.Operations.Services.Frm;
    using ESFA.DC.Web.Operations.Services.Hubs;
    using ESFA.DC.Web.Operations.Services.PeriodEnd;
    using ESFA.DC.Web.Operations.Services.Provider;
    using ESFA.DC.Web.Operations.Services.Storage;
    using ESFA.DC.Web.Operations.Settings.Models;
    using Microsoft.EntityFrameworkCore;

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

            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();

            builder.RegisterType<HttpClient>().SingleInstance();
            builder.RegisterType<EmailDistributionService>().As<IEmailDistributionService>().InstancePerLifetimeScope();
            builder.RegisterType<EmailService>().As<IEmailService>().InstancePerLifetimeScope();
            builder.RegisterType<HistoryService>().As<IHistoryService>().InstancePerLifetimeScope();
            builder.RegisterType<StateService>().As<IStateService>().InstancePerLifetimeScope();

            builder.RegisterType<PeriodEndStateFactory>().As<IPeriodEndStateFactory>().InstancePerLifetimeScope();
            builder.RegisterType<AddNewProviderService>().As<IAddNewProviderService>().InstancePerLifetimeScope();
            builder.RegisterType<ManageProvidersService>().As<IManageProvidersService>().InstancePerLifetimeScope();
            builder.RegisterType<ManageAssignmentsService>().As<IManageAssignmentsService>().InstancePerLifetimeScope();
            builder.RegisterType<FrmService>().As<IFrmService>().InstancePerLifetimeScope();

            // DB Contexts
            builder.RegisterType<Topics.Data.JobQueueDataContext>().SingleInstance();
            builder.RegisterType<JobQueueDataContext>().As<IJobQueueDataContext>().ExternallyOwned();
            builder.RegisterType<OrganisationsContext>().As<IOrganisationsContext>().ExternallyOwned();

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

            builder.Register(context =>
                {
                    var config = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<JobQueueDataContext>();
                    optionsBuilder.UseSqlServer(
                        config.JobManagement,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<JobQueueDataContext>>()
                .SingleInstance();

            builder.Register(context =>
                {
                    var config = context.Resolve<ConnectionStrings>();
                    var optionsBuilder = new DbContextOptionsBuilder<OrganisationsContext>();
                    optionsBuilder.UseSqlServer(
                        config.Org,
                        options => options.EnableRetryOnFailure(3, TimeSpan.FromSeconds(3), new List<int>()));

                    return optionsBuilder.Options;
                })
                .As<DbContextOptions<OrganisationsContext>>()
                .SingleInstance();
        }
    }
}