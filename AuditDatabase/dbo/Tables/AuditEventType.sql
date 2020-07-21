CREATE TABLE [dbo].[AuditEventType]
(	[ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [EventId] [int] NOT NULL,
    [EventTitle] [nvarchar](max) NOT NULL,
    [EventDescription] [nvarchar](max) NOT NULL, 
    [EventOperationType] NVARCHAR(MAX) NOT NULL
) 
GO
