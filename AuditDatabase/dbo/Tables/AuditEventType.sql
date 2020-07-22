CREATE TABLE [dbo].[AuditEventType]
(	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [EventId] [int] NOT NULL,
    [EventTitle] VARCHAR(50) NOT NULL,
    [EventDescription] [nvarchar](500) NOT NULL, 
    [EventOperationType] NVARCHAR(50) NOT NULL
) 
GO
