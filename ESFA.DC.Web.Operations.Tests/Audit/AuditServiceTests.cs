using Autofac;
using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.JobQueueManager;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Serialization.Json;
using ESFA.DC.Web.Operations.Interfaces.Auditing;
using ESFA.DC.Web.Operations.Models.Auditing;
using ESFA.DC.Web.Operations.Models.Auditing.DTOs.FRM;
using ESFA.DC.Web.Operations.Models.Auditing.DTOs.Provider;
using ESFA.DC.Web.Operations.Services.Auditing;
using ESFA.DC.Web.Operations.Topics.Data.Auditing;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using MoreLinq.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Fabric.Management.ServiceModel;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace ESFA.DC.Web.Operations.Tests.Audit
{
    public class AuditServiceTests
    {
        [Fact]
        public async void testSaveAuditWithoutOld()
        {
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var serializationServiceMock = new Mock<IJsonSerializationService>();
            var auditRepositoryMock = new Mock<IAuditRepository>();
            var auditServiceMock = new Mock<IAuditService>();
            var differentiatorLookupServiceMock = new Mock<IDifferentiatorLookupService>();
            var dateTime = new DateTime(2019, 1, 2);
            var user = "user";
            var cancellationToken = CancellationToken.None;          
            var newDTO = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
            dateTimeProviderMock.Setup(p => p.GetNowUtc()).Returns(dateTime);
            serializationServiceMock.Setup(p => p.Serialize(newDTO)).Returns("{\"CollectionType\":\"ILR1920\",\"StartDateUTC\":\"9999-12-31T23:59:59.9999999\",\"EndDateUTC\":\"9999-12-31T23:59:59.9999999\"}");
            differentiatorLookupServiceMock.Setup(p => p.DifferentiatorLookup<AmendCollectionDTO>()).Returns(20);
            var service = new AuditService(dateTimeProviderMock.Object, serializationServiceMock.Object, auditRepositoryMock.Object, differentiatorLookupServiceMock.Object);


            await service.CreateAuditAsync<AmendCollectionDTO>(user, newDTO, cancellationToken);


            auditRepositoryMock.Verify(r => r.SaveAuditAsync(user, dateTime, differentiatorLookupServiceMock.Object.DifferentiatorLookup<AmendCollectionDTO>(), serializationServiceMock.Object.Serialize(newDTO), null, cancellationToken));

        }
        [Fact]
        public async void testSaveAuditWithOld()
        {
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var serializationServiceMock = new Mock<IJsonSerializationService>();
            var auditRepositoryMock = new Mock<IAuditRepository>();
            var differentiatorLookupServiceMock = new Mock<IDifferentiatorLookupService>();
            var dateTime = new DateTime(2019, 1, 2);
            var user = "User";
            var cancellationToken = CancellationToken.None;           
            var newDTO = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
            var oldDTO = new AmendCollectionDTO { CollectionType = "ILR2021", StartDateUTC = DateTime.MinValue, EndDateUTC = DateTime.MinValue };
            dateTimeProviderMock.Setup(p => p.GetNowUtc()).Returns(dateTime);
            serializationServiceMock.Setup(p => p.Serialize(newDTO)).Returns("{\"CollectionType\":\"ILR1920\",\"StartDateUTC\":\"9999-12-31T23:59:59.9999999\",\"EndDateUTC\":\"9999-12-31T23:59:59.9999999\"}");
            serializationServiceMock.Setup(p => p.Serialize(oldDTO)).Returns("{\"CollectionType\":\"ILR2021\",\"StartDateUTC\":\"0001-01-01T00:00:00\",\"EndDateUTC\":\"0001-01-01T00:00:00\"}");
            differentiatorLookupServiceMock.Setup(p => p.DifferentiatorLookup<AmendCollectionDTO>()).Returns(20);
            var service = new AuditService(dateTimeProviderMock.Object, serializationServiceMock.Object, auditRepositoryMock.Object, differentiatorLookupServiceMock.Object);


            await service.CreateAuditAsync<AmendCollectionDTO>(user, newDTO, oldDTO, cancellationToken);


            auditRepositoryMock.Verify(r => r.SaveAuditAsync(user, dateTime, differentiatorLookupServiceMock.Object.DifferentiatorLookup<AmendCollectionDTO>(), serializationServiceMock.Object.Serialize(newDTO), serializationServiceMock.Object.Serialize(oldDTO), cancellationToken));

        }
    }
}
