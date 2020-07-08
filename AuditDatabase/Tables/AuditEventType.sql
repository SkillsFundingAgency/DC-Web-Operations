CREATE TABLE [dbo].[AuditEventType](
    [Id] [int] IDENTITY(1,1),
    [EventId] [int] NOT NULL,
    [EventTitle] [nvarchar](max) NOT NULL,
    [EventDescription] [nvarchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuditEventType] ADD PRIMARY KEY CLUSTERED 
(
    [Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
