using Autofac;
using DC.Web.Authorization.Data.Repository;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Web.Operations.Interfaces.PeriodEnd;
using ESFA.DC.Web.Operations.Services.Hubs;
using ESFA.DC.Web.Operations.Services.PeriodEnd;

namespace ESFA.DC.Web.Operations.Ioc
{
    public class ServiceRegistrations : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthorizeRepository>().As<IAuthorizeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<PeriodEndService>().As<IPeriodEndService>().InstancePerLifetimeScope();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>().InstancePerLifetimeScope();

            //builder.RegisterType<PeriodEndHub>().As<IPeriodEndHub>().InstancePerLifetimeScope().SingleInstance().ExternallyOwned();
        }
    }
}