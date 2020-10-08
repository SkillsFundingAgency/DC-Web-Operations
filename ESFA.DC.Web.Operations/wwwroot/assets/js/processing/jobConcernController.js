import { getFormattedDatetimeString, replaceNullOrEmpty } from '/assets/js/util.js';
import JobReportControllerBase from './jobReportControllerBase.js';

class JobConcernController extends JobReportControllerBase {

    constructor() {
        super({ hubUrl: 'jobConcernHub'});
    }

    formatDataForDisplay() {
        this._data.jobs.map(item => {
            item.providerName = replaceNullOrEmpty(item.providerName, 'ESFA'),
            item.fileName = replaceNullOrEmpty(item.fileName, ''),
            item.lastSuccessfulSubmission = getFormattedDatetimeString(item.lastSuccessfulSubmission)
        });
    }

    createReportRow(item) {
        return `<tr class="govuk-table__row">
                    <td class="govuk-table__cell">${item.providerName}</td>
                    <td class="govuk-table__cell">${item.ukprn}</td>
                    <td class="govuk-table__cell">${item.fileName}</td>
                    <td class="govuk-table__cell">${item.lastSuccessfulSubmission}</td>
                    <td class="govuk-table__cell">${item.periodOfLastSuccessfulSubmission}</td>
               </tr>`;
    }

    getCSVData() {
        const data = this._data.jobs.map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Filename": obj.fileName,
                "Last successful submission": obj.lastSuccessfulSubmission,
                "Period of last successful submission": obj.periodOfLastSuccessfulSubmission
            };
        });

        return { data, fileName: 'Jobs-Concern.csv' };
    }
}

export const jobConcernController = new JobConcernController();