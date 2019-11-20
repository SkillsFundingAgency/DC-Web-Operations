﻿namespace ESFA.DC.Web.Operations.Utils
{
    public class Constants
    {
        public const string IlrCollectionNamePrefix = "ILR";
        public const int YearStartMonth = 8;

        public const int CriticalPathHubId = 0;

        public const string CollectionYearToken = "{collectionYear}";
        public static readonly string PeriodEndBlobContainerName = "periodend" + CollectionYearToken + "-files";

        public const string ReferenceDataJobPausedState = "Paused";
        public const string ReferenceDataJobDisabledState = "Disabled";

        public const string Action_ReferenceJobsButton = "ReferenceJobsButtonState";
        public const string Action_CollectionClosedEmailButton = "CollectionClosedEmailButtonState";
        public const string Action_ContinueButton = "ContinueButtonState";

        public const string Action_StartPeriodEndButton = "StartPeriodEndState";
        public const string Action_MCAReportsButton = "MCAReportsState";
        public const string Action_ProviderReportsButton = "ProviderReportsState";
        public const string Action_PeriodClosedButton = "PeriodClosedState";
    }
}
