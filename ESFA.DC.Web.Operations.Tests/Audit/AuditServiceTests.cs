﻿using ESFA.DC.DateTimeProvider.Interface;
using ESFA.DC.Serialization.Interfaces;
using ESFA.DC.Web.Operations.Interfaces.Auditing;
using ESFA.DC.Web.Operations.Models.Auditing.DTOs.Provider;
using ESFA.DC.Web.Operations.Services.Auditing;
using Moq;
using System;
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
            var differentiator = 20;
            var cancellationToken = CancellationToken.None;          
            var newDTO = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
            dateTimeProviderMock.Setup(p => p.GetNowUtc()).Returns(dateTime);
            serializationServiceMock.Setup(p => p.Serialize(newDTO)).Returns("{\"CollectionType\":\"ILR1920\",\"StartDateUTC\":\"9999-12-31T23:59:59.9999999\",\"EndDateUTC\":\"9999-12-31T23:59:59.9999999\"}");
            differentiatorLookupServiceMock.Setup(p => p.DifferentiatorLookup<AmendCollectionDTO>()).Returns(differentiator);
            var service = new AuditService(dateTimeProviderMock.Object, serializationServiceMock.Object, auditRepositoryMock.Object, differentiatorLookupServiceMock.Object);


            await service.CreateAuditAsync<AmendCollectionDTO>(user, newDTO, cancellationToken);


            auditRepositoryMock.Verify(r => r.SaveAuditAsync(user, dateTime, differentiator, serializationServiceMock.Object.Serialize(newDTO), null, cancellationToken));

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
            var differentiator = 20;
            var cancellationToken = CancellationToken.None;           
            var newDTO = new AmendCollectionDTO { CollectionType = "ILR1920", StartDateUTC = DateTime.MaxValue, EndDateUTC = DateTime.MaxValue };
            var oldDTO = new AmendCollectionDTO { CollectionType = "ILR2021", StartDateUTC = DateTime.MinValue, EndDateUTC = DateTime.MinValue };
            dateTimeProviderMock.Setup(p => p.GetNowUtc()).Returns(dateTime);
            serializationServiceMock.Setup(p => p.Serialize(newDTO)).Returns("{\"CollectionType\":\"ILR1920\",\"StartDateUTC\":\"9999-12-31T23:59:59.9999999\",\"EndDateUTC\":\"9999-12-31T23:59:59.9999999\"}");
            serializationServiceMock.Setup(p => p.Serialize(oldDTO)).Returns("{\"CollectionType\":\"ILR2021\",\"StartDateUTC\":\"0001-01-01T00:00:00\",\"EndDateUTC\":\"0001-01-01T00:00:00\"}");
            differentiatorLookupServiceMock.Setup(p => p.DifferentiatorLookup<AmendCollectionDTO>()).Returns(20);
            var service = new AuditService(dateTimeProviderMock.Object, serializationServiceMock.Object, auditRepositoryMock.Object, differentiatorLookupServiceMock.Object);


            await service.CreateAuditAsync<AmendCollectionDTO>(user, newDTO, oldDTO, cancellationToken);


            auditRepositoryMock.Verify(r => r.SaveAuditAsync(user, dateTime, differentiator, serializationServiceMock.Object.Serialize(newDTO), serializationServiceMock.Object.Serialize(oldDTO), cancellationToken));

        }
    }
}
