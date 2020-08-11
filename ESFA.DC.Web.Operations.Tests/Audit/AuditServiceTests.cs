using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Web.Operations.Interfaces.Auditing;
using ESFA.DC.Web.Operations.Models.Auditing.DTOs.FRM;
using ESFA.DC.Web.Operations.Models.Auditing.DTOs.Provider;
using ESFA.DC.Web.Operations.Services.Auditing;
using ESFA.DC.Web.Operations.Topics.Data.Auditing;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace ESFA.DC.Web.Operations.Tests.Audit
{
    public class AuditServiceTests
    {
        #region DB count tests
        [Fact]
        public async void testNoNewValueSaved()
        {
            //test that a record is added to the db using the no NewValue overloads
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    var testDTO = new FrmPublishDTO { JobID = 3 };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<FrmPublishDTO>(Environment.UserName ?? "username", testDTO, CancellationToken.None);
                    var count = context.Audit.Count();

                    count.Should().Be(1);
                }
            }
        }

        [Fact]
        public async void testContainsNewValueSaved()
        {
            //test that a record is added to the db using the NewValue overloads
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    var newTestDTO = new FrmPublishDTO { JobID = 3 };
                    var oldTestDTO = new FrmPublishDTO { JobID = 4 };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<FrmPublishDTO>(Environment.UserName ?? "username", newTestDTO, oldTestDTO, CancellationToken.None);
                    var count = context.Audit.Count();

                    count.Should().Be(1);
                }
            }
        }
        #endregion

        #region Username Tests
        [Fact]

        public async void testSaveUser()
        {
            //testing username saves properly with no newValue overload
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    var testDTO = new FrmPublishDTO { JobID = 3 };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<FrmPublishDTO>("David", testDTO, CancellationToken.None);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var user = audit.User;

                    user.Should().Be("David");
                }
            }
        }

        [Fact]
        public async void testSaveUserContainsNew()
        {
            //testing username saves properly with newValue overload
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    var newTestDTO = new FrmPublishDTO { JobID = 3 };
                    var oldTestDTO = new FrmPublishDTO { JobID = 4 };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<FrmPublishDTO>("David", newTestDTO, oldTestDTO, CancellationToken.None);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var user = audit.User;

                    user.Should().Be("David");
                }
            }
        }
        #endregion

        #region AuditValue Tests
        [Fact]

        public async void testSaveAuditValueTrue()
        {
            //testing auditValue with no newValue overload
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    var dto = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<AmendCollectionDTO>("David", dto, CancellationToken.None);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.NewValue;

                    auditValue.Should().Be("{\"CollectionType\":\"ILR1920\",\"StartDateUTC\":\"9999-12-31T23:59:59.9999999\",\"EndDateUTC\":\"9999-12-31T23:59:59.9999999\"}");
                }
            }
        }
        [Fact]
        public async void testSaveAuditValueFalse()
        {
            //testing auditValue with no newValue overload
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    var dto = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<AmendCollectionDTO>("David", dto, CancellationToken.None);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.NewValue;

                    auditValue.Should().NotBe("{\"CollectionType\":\"ESF\",\"StartDateUTC\":\"9999-12-31T23:59:59.9999999\",\"EndDateUTC\":\"9999-12-31T23:59:59.9999999\"}");
                }
            }
        }

        [Fact]
        public async void testSaveAuditNewValueTrue()
        {
            //testing auditValue with newValue overload
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    //create object for newValue
                    var newDTO = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
                    var oldDTO = new AmendCollectionDTO { CollectionType = "ILR2021", StartDateUTC = DateTime.MinValue, EndDateUTC = DateTime.MinValue };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<AmendCollectionDTO>("David", newDTO, oldDTO, CancellationToken.None);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.NewValue;

                    auditValue.Should().Be("{\"CollectionType\":\"ILR1920\",\"StartDateUTC\":\"9999-12-31T23:59:59.9999999\",\"EndDateUTC\":\"9999-12-31T23:59:59.9999999\"}");
                }
            }
        }
        [Fact]
        public async void testSaveAuditNewValueFalse()
        {
            //testing auditValue with newValue overload
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    //create object for newValue
                    var newDTO = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
                    var oldDTO = new AmendCollectionDTO { CollectionType = "ILR2021", StartDateUTC = DateTime.MinValue, EndDateUTC = DateTime.MinValue };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<AmendCollectionDTO>("David", newDTO, oldDTO, CancellationToken.None);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.NewValue;

                    auditValue.Should().NotBe("{\"CollectionType\":\"ESF\",\"StartDateUTC\":\"9999-12-31T23:59:59.9999999\",\"EndDateUTC\":\"9999-12-31T23:59:59.9999999\"}");
                }
            }
        }

        [Fact]
        public async void testSaveAuditOldValueTrue()
        {
            //testing auditValue with newValue overload
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    //create object for newValue
                    var newDTO = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
                    var oldDTO = new AmendCollectionDTO { CollectionType = "ILR2021", StartDateUTC = DateTime.MinValue, EndDateUTC = DateTime.MinValue };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<AmendCollectionDTO>("David", newDTO, oldDTO, CancellationToken.None);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.OldValue;

                    auditValue.Should().Be("{\"CollectionType\":\"ILR2021\",\"StartDateUTC\":\"0001-01-01T00:00:00\",\"EndDateUTC\":\"0001-01-01T00:00:00\"}");
                }
            }
        }
        [Fact]
        public async void testSaveAuditOldValueFalse()
        {
            //testing auditValue with newValue overload
            var container = getRegistrations();
            using (var scope = container.BeginLifetimeScope())
            {
                // Create the schema in the database
                var options = scope.Resolve<DbContextOptions<AuditDataContext>>();
                using (var context = new AuditDataContext(options))
                {
                    setUpInMemoryDB(context);
                    //create object for newValue
                    var newDTO = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
                    var oldDTO = new AmendCollectionDTO { CollectionType = "ILR2021", StartDateUTC = DateTime.MinValue, EndDateUTC = DateTime.MinValue };
                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAuditAsync<AmendCollectionDTO>("David", newDTO, oldDTO, CancellationToken.None);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.OldValue;

                    auditValue.Should().NotBe("{\"CollectionType\":\"ESF\",\"StartDateUTC\":\"0001-01-01T00:00:00\",\"EndDateUTC\":\"0001-01-01T00:00:00\"}");
                }
            }
        }


        #endregion

        #region privateFunctions
        private void setUpInMemoryDB(AuditDataContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        private IContainer getRegistrations()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterType<AuditService>().As<IAuditService>();
            builder.RegisterType<AuditDataContext>().As<IAuditDataContext>();
            builder.RegisterType<DateTimeProvider.DateTimeProvider>().As<IDateTimeProvider>();
            builder.RegisterType<JsonSerializationService>().As<IJsonSerializationService>();
            builder.Register(context =>
            {
                SqliteConnection connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();
                DbContextOptionsBuilder<AuditDataContext> optionsBuilder =
                    new DbContextOptionsBuilder<AuditDataContext>()
                        .UseSqlite(connection);

                return optionsBuilder.Options;
            })
                    .As<DbContextOptions<AuditDataContext>>()
                    .SingleInstance();
            return builder.Build();
        }
        #endregion
    }
}
