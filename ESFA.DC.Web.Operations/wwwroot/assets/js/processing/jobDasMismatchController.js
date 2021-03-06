﻿import { getFormattedDatetimeString, replaceNullOrEmpty } from '/assets/js/util.js';
import { sortByProviderName } from '/assets/js/sortingUtils.js';
import JobReportControllerBase from './jobReportControllerBase.js';

class JobDasMismatchController extends JobReportControllerBase {

    constructor() {
        super({ hubUrl: 'jobDasMismatchHub', defaultSort: sortByProviderName });
    }

    formatDataForDisplay() {
        this._data.jobs.map(item => {
            item.providerName = replaceNullOrEmpty(item.providerName, 'ESFA'),
            item.fileName = replaceNullOrEmpty(item.fileName, ''),
            item.submissionDate = getFormattedDatetimeString(item.submissionDate)
        });
    }

    createReportRow(item) {
        return `<tr class="govuk-table__row">
                    <td class="govuk-table__cell">${item.providerName}</td>
                    <td class="govuk-table__cell">${item.ukprn}</td>
                    <td class="govuk-table__cell">${item.jobId}</td>
                    <td class="govuk-table__cell">${item.fileName}</td>
                    <td class="govuk-table__cell">${item.submissionDate}</td>
                    <td class="govuk-table__cell">${item.jobId}</td>
                </tr>`;
    }

    getCSVData() {
        const data = this._data.jobs.map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Job Id": obj.jobId,
                "Filename": obj.fileName,
                "Date/time of submission": obj.submissionDate,
                "Job id": obj.jobId
            };
        });

        return { data, fileName: 'Jobs-DasMismatch.csv'};
    }
}

export const jobDasMismatchController = new JobDasMismatchController();