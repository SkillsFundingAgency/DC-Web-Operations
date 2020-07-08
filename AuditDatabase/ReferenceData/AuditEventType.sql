/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
BEGIN

	DECLARE @SummaryOfChanges_AuditEventType TABLE (EventId int, EventTitle nvarchar(MAX), EventDescription nvarchar(MAX) , [Action] VARCHAR(100));

	MERGE INTO [dbo].[AuditEventType] AS Target
	USING (VALUES
			(0, 'FailJob','') ,
        (1, 'ManagingPeriodCollection', ''),
        (2, 'SelectOverride', ''),
        (3, 'FrmPublish',''),
        (4, 'FrmUnpublish', ''),
        (5, 'FrmValidate', ''),
        (6, 'AddSpecificNotification', ''),
        (7, 'AmendNotifications', ''),
        (8, 'AmendGroup',''),
        (9, 'AmendPeriodEnd',''),
        (10, 'AmendRecipient',''),
        (11, 'ClosedCollectionEmail',''),
        (12, 'ClosePeriodEnd',''),
        (13, 'EditEmailTemplate',''),
        (14, 'PublishMCAReports',''),
        (15, 'PublishProviderReports', ''),
        (16, 'ReferenceDataButtons', ''),
        (17, 'ResubmitFailedJob', ''),
        (18, 'SaveValidityChanges',''),
        (19, 'AddNewProvider',''),
        (20, 'AmendCollection',''),
        (21,'ChangeProviderName',''),
        (22,'ChangeProviderUKPRN',''),
        (23,'ChangeProviderUPIN',''),
        (24,'EditProviderDetails',''),
        (25,'EditProvidersAssignments',''),
        (26,'ProviderUploadFile',''),
        (27,'AddNewClaimsDates',''),
        (28,'ModifyClaimDates',''),
        (29,'ReferenceDataUploadFile',''))		  
		AS Source(EventId, EventTitle, EventDescription)
		ON Target.EventId = Source.EventId
		WHEN MATCHED 
				AND EXISTS 
					(		SELECT Target.EventId
								  ,Target.EventTitle
								  ,Target.EventDescription
						EXCEPT 
							SELECT Source.EventId
								  ,Source.EventTitle
								  ,Source.EventDescription
					)
			  THEN UPDATE SET Target.EventId = Source.EventId,
							  Target.EventTitle = Source.EventTitle,
							  Target.EventDescription = Source.EventDescription
		WHEN NOT MATCHED BY TARGET THEN INSERT(EventId, EventTitle, EventDescription) 
									   VALUES (EventId, EventTitle, EventDescription)
		WHEN NOT MATCHED BY SOURCE THEN DELETE
		OUTPUT Inserted.EventId,$action INTO @SummaryOfChanges_AuditEventType(EventId,[Action])
	;

		DECLARE @AddCount_CT INT, @UpdateCount_CT INT, @DeleteCount_CT INT
		SET @AddCount_CT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_AuditEventType WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_AuditEventType WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_AuditEventType WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		        %s - Added %i - Update %i - Delete %i',10,1,'    AuditEventType', @AddCount_CT, @UpdateCount_CT, @DeleteCount_CT) WITH NOWAIT;
END
GO