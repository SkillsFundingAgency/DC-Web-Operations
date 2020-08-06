using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager;
using ESFA.DC.Web.Operations.Interfaces.Auditing;
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

                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR1920"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYear", DateTime.MaxValue),
            };


                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAudit(keyValues, Environment.UserName ?? "username", 1);
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

                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR1920"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYear", DateTime.MaxValue),
            };

                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAudit(keyValues, keyValues, Environment.UserName ?? "username", 1);
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

                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR1920"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYear", DateTime.MaxValue),
            };



                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAudit(keyValues, "David", 1);
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

                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR1920"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYear", DateTime.MaxValue),
            };



                    var service = scope.Resolve<IAuditService>();

                    await service.CreateAudit(keyValues, keyValues, "David", 1);
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

                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR1920"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYear", DateTime.MaxValue),
            };

                    var service = scope.Resolve<IAuditService>();
                    await service.CreateAudit(keyValues, "David", 1);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.NewValue;
                    auditValue.Should().Be("{\r\n  \"CollectionName\": \"ILR1920\",\r\n  \"StartDateUTC\": \"9999-12-31T23:59:59.9999999\",\r\n  \"CollectionYear\": \"9999-12-31T23:59:59.9999999\"\r\n}");
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

                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR2020"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYears", DateTime.MaxValue),
            };


                    //create second object for comparing
                    var keyValues2 = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR2021"),
                new Tuple<string, object>("StartDateUTC", DateTime.MinValue),
                new Tuple<string, object>("CollectionYears", DateTime.MinValue),
            };

                    dynamic obj2 = new JObject();

                    foreach (var keyValue in keyValues2)
                    {
                        obj2[keyValue.Item1] = JToken.FromObject(keyValue.Item2);
                    }
                    var service = scope.Resolve<IAuditService>();
                    await service.CreateAudit(keyValues, "David", 1);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.NewValue;
                    bool result = true;
                    result = Equals(auditValue, obj2.ToString());
                    result.Should().Be(false);
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
                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR2020"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYears", DateTime.MaxValue),
            };

                    //create second object for oldValue and comparison
                    var keyValues2 = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR2021"),
                new Tuple<string, object>("StartDateUTC", DateTime.MinValue),
                new Tuple<string, object>("CollectionYears", DateTime.MinValue),
            };

                    dynamic oldValue = new JObject();

                    foreach (var keyValue in keyValues2)
                    {
                        oldValue[keyValue.Item1] = JToken.FromObject(keyValue.Item2);
                    }

                    var service = scope.Resolve<IAuditService>();
                    await service.CreateAudit(keyValues, keyValues2, "David", 1);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.NewValue;
                    bool result = true;
                    result = Equals(auditValue, oldValue.ToString());
                    result.Should().Be(false);
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
                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR1920"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYears", DateTime.MaxValue),
            };


                    //create second object for oldValue
                    var keyValues2 = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR2021"),
                new Tuple<string, object>("StartDateUTC", DateTime.MinValue),
                new Tuple<string, object>("CollectionYears", DateTime.MinValue),
            };

                    dynamic oldValue = new JObject();

                    foreach (var keyValue in keyValues2)
                    {
                        oldValue[keyValue.Item1] = JToken.FromObject(keyValue.Item2);
                    }

                    var service = scope.Resolve<IAuditService>();
                    await service.CreateAudit(keyValues, keyValues2, "David", 1);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.NewValue;
                    bool result = Equals(auditValue, "{\r\n  \"CollectionName\": \"ILR1920\",\r\n  \"StartDateUTC\": \"9999-12-31T23:59:59.9999999\",\r\n  \"CollectionYears\": \"9999-12-31T23:59:59.9999999\"\r\n}");
                    result.Should().Be(true);
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
                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR2020"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYears", DateTime.MaxValue),
            };


                    //create second object for oldValue
                    var keyValues2 = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR2021"),
                new Tuple<string, object>("StartDateUTC", DateTime.MinValue),
                new Tuple<string, object>("CollectionYears", DateTime.MinValue),
            };



                    var service = scope.Resolve<IAuditService>();
                    await service.CreateAudit(keyValues, keyValues2, "David", 1);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.OldValue;
                    bool result = Equals(auditValue, "{\r\n  \"CollectionName\": \"ILR2021\",\r\n  \"StartDateUTC\": \"0001-01-01T00:00:00\",\r\n  \"CollectionYears\": \"0001-01-01T00:00:00\"\r\n}");
                    result.Should().Be(true);
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
                    //create object for newValue and comparison
                    var keyValues = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR1920"),
                new Tuple<string, object>("StartDateUTC", DateTime.MaxValue),
                new Tuple<string, object>("CollectionYears", DateTime.MaxValue),
            };



                    //create second object for oldValue
                    var keyValues2 = new List<Tuple<string, object>>()
            {
                new Tuple<string, object>("CollectionName", "ILR2021"),
                new Tuple<string, object>("StartDateUTC", DateTime.MinValue),
                new Tuple<string, object>("CollectionYears", DateTime.MinValue),
            };

                    dynamic oldValue = new JObject();

                    foreach (var keyValue in keyValues2)
                    {
                        oldValue[keyValue.Item1] = JToken.FromObject(keyValue.Item2);
                    }

                    var service = scope.Resolve<IAuditService>();
                    await service.CreateAudit(keyValues, keyValues2, "David", 1);
                    var audit = context.Audit.SingleOrDefault(i => i.Id == 1);
                    var auditValue = audit.OldValue;
                    bool result = true;
                    result = Equals(auditValue, "{\r\n  \"CollectionName\": \"ILR1920\",\r\n  \"StartDateUTC\": \"9999-12-31T23:59:59.9999999\",\r\n  \"CollectionYear\": \"9999-12-31T23:59:59.9999999\"\r\n}");
                    result.Should().Be(false);
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
