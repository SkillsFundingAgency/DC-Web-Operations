import { getFormattedDatetimeString, getFormattedTimeString, replaceNullOrEmpty } from '/assets/js/util.js';
import { sortByProviderName } from '/assets/js/sortingUtils.js';
import JobReportControllerBase from './jobReportControllerBase.js';

class JobFailedCurrentPeriodController extends JobReportControllerBase {

    constructor() {
        super({ hubUrl: 'jobFailedCurrentPeriodHub', defaultSort: sortByProviderName });
    }

    formatDataForDisplay() {
        this._data.jobs.map(item => {
            item.providerName = replaceNullOrEmpty(item.providerName, 'ESFA'),
            item.fileName = replaceNullOrEmpty(item.fileName, ''),
            item.dateTimeOfFailure = getFormattedDatetimeString(item.dateTimeOfFailure),
            item.processingTimeBeforeFailure = getFormattedTimeString(item.processingTimeBeforeFailure)
        });
    }

    createReportRow(item) {
        return `<tr class="govuk-table__row">
                    <td class="govuk-table__cell">${item.providerName}</td>
                    <td class="govuk-table__cell">${item.ukprn}</td>
                    <td class="govuk-table__cell">${item.fileName}</td>
                    <td class="govuk-table__cell">${item.dateTimeOfFailure}</td>
                    <td class="govuk-table__cell">${item.processingTimeBeforeFailure}</td>
                 </tr>`
    }

    getCSVData() {
        const data = this._data.jobs.map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Filename": obj.fileName,
                "Date/time of failure": obj.dateTimeOfFailure,
                "Processing time before failure": obj.processingTimeBeforeFailure
            };
        });

        return { data, fileName: 'Jobs-FailedInCurrentPeriod.csv' };
    }
}

export const jobFailedCurrentPeriodController = new JobFailedCurrentPeriodController();