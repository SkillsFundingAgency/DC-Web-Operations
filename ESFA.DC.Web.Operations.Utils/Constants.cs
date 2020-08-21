﻿using System;

namespace ESFA.DC.Web.Operations.Utils
{
    public class Constants
    {
        public const int MaxFilesToDisplay = 15;
        public const int ValidationMessagesMaxFilesToDisplay = 25;

        public const string IlrCollectionNamePrefix = "ILR";
        public const int YearStartMonth = 8;
        public const int CriticalPathHubId = 0;
        public const string CollectionYearToken = "{collectionYear}";
        public const string ReferenceDataJobPausedState = "Paused";
        public const string ReferenceDataJobDisabledState = "Disabled";
        public const string Action_ReferenceJobsButton = "ReferenceJobsButtonState";
        public const string Action_CollectionClosedEmailButton = "CollectionClosedEmailButtonState";
        public const string Action_ContinueButton = "ContinueButtonState";
        public const string Action_StartPeriodEndButton = "StartPeriodEndState";
        public const string Action_MCAReportsButton = "MCAReportsState";
        public const string Action_ProviderReportsButton = "ProviderReportsState";
        public const string Action_PeriodClosedButton = "PeriodClosedState";
        public const string Action_UploadFileButton = "UploaddState";

        public const string PeriodEndDataQualityReportCollectionName = @"PE-DataQuality-Report" + CollectionYearToken;
        public const string ProviderSubmissionsReportCollectionName = @"PE-ProviderSubmissions-Report" + CollectionYearToken;
        public const string PeriodEndMetricsReportCollectionName = @"PE-Metrics-Report" + CollectionYearToken;
        public const string PeriodEndDataExtractReportCollectionName = @"PE-DataExtract-Report" + CollectionYearToken;
        public const string InternalDataMatchReportCollectionName = @"PE-DAS-AppsInternalDataMatchMonthEndReport" + CollectionYearToken;
        public const string ActCountReportCollectionName = @"PE-ACT-Count-Report" + CollectionYearToken;
        public const string ILRProvidersReturningFirstTimePerDayReport = @"OP-ILRProvidersReturningFirstTimePerDay-Report";

        public const string FrmReportCollectionName = @"FRM-Reports1920";

        public const string ValidationRuleDetailsReportCollectionName = @"OP-VALIDATION-REPORT";

        public const string ALLFStorageContainerName = "allf-files";
        public const string ReferenceDataStorageContainerName = "reference-data";

        public static readonly string PeriodEndBlobContainerName = "periodend" + CollectionYearToken + "-files";
        public static readonly string ReportsBlobContainerName = "reports" + CollectionYearToken + "-files";

        public static readonly string OpsReportsBlobContainerName = "opsreferencedata";

        public static readonly string FrmContainerName = "frm{0}-files";
        public static readonly string FrmValidationKey = "Validation";
        public static readonly string FrmPublishKey = "Publish";

        public static readonly string DASSubmission = @"PE-DAS-Submission";

        public static readonly string IlrPeriodPrefix = "R";
        public static readonly string NcsPeriodPrefix = "N";
        public static readonly string ALLFPeriodPrefix = "A";

        public static readonly DateTime MaxDateTime = new DateTime(2600, 7, 31);
    }
}
