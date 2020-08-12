using System;
using System.Collections.Generic;
using System.Text;

namespace ESFA.DC.Web.Operations.Models.Auditing
{
    public enum DifferentiatorPath
    {
        FailJobDTO = 0,
        ManagingPeriodCollectionDTO = 1,
        SelectOverrideDTO = 2,
        FrmPublishDTO = 3,
        FrmUnpublishDTO = 4,
        FrmValidateDTO = 5,
        AddSpecificNotificationDTO = 6,
        AmendNotificationsDTO = 7,
        AmendGroupDTO = 8,
        AmendPeriodEndDTO = 9,
        AmendRecipientDTO = 10,
        ClosedCollectionEmailDTO = 11,
        ClosePeriodEndDTO = 12,
        EditEmailTemplateDTO = 13,
        PublishMCAReportsDTO = 14,
        PublishProviderReportsDTO = 15,
        ReferenceDataButtonsDTO = 16,
        ResubmitFailedJobDTO = 17,
        SaveValidityChangesDTO = 18,
        AddNewProviderDTO = 19,
        AmendCollectionDTO = 20,
        ChangeProviderNameDTO = 21,
        ChangeProviderUKPRNDTO = 22,
        ChangeProviderUPINDTO = 23,
        EditProviderDetailsDTO = 24,
        EditProvidersAssignmentsDTO = 25,
        ProviderUploadFileDTO = 26,
        AddNewClaimsDatesDTO = 27,
        ModifyClaimDatesDTO = 28,
        ReferenceDataUploadFileDTO = 29,
    }
}