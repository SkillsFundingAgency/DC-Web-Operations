import { getFormattedDatetimeString, getFormattedTimeString, replaceNullOrEmpty } from '/assets/js/util.js';
import { sortByProviderName } from '/assets/js/sortingUtils.js';
import JobReportControllerBase from './jobReportControllerBase.js';

class JobProvidersReturnedCurrentPeriodController extends JobReportControllerBase {

    constructor() {
        super({ hubUrl: 'jobProvidersReturnedCurrentPeriodHub', defaultSort: sortByProviderName});
    }

    formatDataForDisplay() {
        this._data.jobs.map(item => {
            item.providerName = replaceNullOrEmpty(item.providerName, 'ESFA'),
            item.fileName = replaceNullOrEmpty(item.fileName, ''),
            item.dateTimeSubmission = getFormattedDatetimeString(item.dateTimeSubmission),
            item.processingTime = getFormattedTimeString(item.processingTime)
        });
    }

    createReportRow(item) {
        return `<tr class="govuk-table__row">
                    <td class="govuk-table__cell">${item.providerName}</td>
                    <td class="govuk-table__cell">${item.ukprn}</td>
                    <td class="govuk-table__cell">${item.jobId}</td>
                    <td class="govuk-table__cell">${item.fileName}</td>
                    <td class="govuk-table__cell">${item.dateTimeSubmission}</td>
                    <td class="govuk-table__cell">${item.processingTime}</td>
                </tr>`;
    }

    getCSVData() {
        const data = this._data.jobs.map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Job Id": obj.jobId,
                "Filename": obj.fileName,
                "Date/time of submission (latest)": obj.dateTimeSubmission,
                "Processing time": obj.processingTime
            };
        });

        return { data, fileName: 'Jobs-ProvidersReturnedCurrentPeriod.csv'};
    }
}

export const jobProvidersReturnedCurrentPeriodController = new JobProvidersReturnedCurrentPeriodController()