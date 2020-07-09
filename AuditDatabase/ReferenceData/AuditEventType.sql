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
			(0, 'FailJob','User selects to fail individual job', 'U') ,
        (1, 'ManagingPeriodCollection', 'Time and date of given collection edited to users choice'),
        (2, 'SelectOverride', 'Override status updated', 'U'),
        (3, 'FrmPublish','User published a period of FRM Reports', 'U'),
        (4, 'FrmUnpublish', 'User unpublished a period of FRM Reports', 'D'),
        (5, 'FrmValidate', 'User validated a period of FRM Reports', 'C'),
        (6, 'AddSpecificNotification', 'User Added a notification', 'CD'),
        (7, 'AmendNotifications', 'Notification amended', 'UD'),
        (8, 'AmendGroup','A Group has been amended', 'UD'),
        (9, 'AmendPeriodEnd','A period has been amended', 'U'),
        (10, 'AmendRecipient','A User is removed from a list', 'U'),
        (11, 'ClosedCollectionEmail','collectionclosed email is sent', 'C'),
        (12, 'ClosePeriodEnd','A period has been closed', 'U'),
        (13, 'EditEmailTemplate','Updated email template', 'U'),
        (14, 'PublishMCAReports','A User has published MCA reports', 'U'),
        (15, 'PublishProviderReports', 'A user has published provider reports', 'U'),
        (16, 'ReferenceDataButtons', 'N/A', 'U'),
        (17, 'ResubmitFailedJob', 'A User has resubmitted a failed job', 'U'),
        (18, 'SaveValidityChanges','A User has made changes to a validity period', 'U'),
        (19, 'AddNewProvider','A new provider has been added', 'C'),
        (20, 'AmendCollection','A provider has had their collections updated', 'UD'),
        (21,'ChangeProviderName','A provider name has been changed', 'U'),
        (22,'ChangeProviderUKPRN','A provider UKPRN has been changed', 'U'),
        (23,'ChangeProviderUPIN','A provider UPIN has been changed', 'U'),
        (24,'EditProviderDetails','A providers details have been edited'),
        (25,'EditProvidersAssignments','A providers assignments have been edited'),
        (26,'ProviderUploadFile','A user has uploaded a file to add multiple providers', 'C'),
        (27,'AddNewClaimsDates','FundingClaimsDates: Add new claim dates', 'CU'),
        (28,'ModifyClaimDates','FundingClaimsDates: User Modified claim dates','U'),
        (29,'ReferenceDataUploadFile','A user has uploaded a file', 'C'))		  
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