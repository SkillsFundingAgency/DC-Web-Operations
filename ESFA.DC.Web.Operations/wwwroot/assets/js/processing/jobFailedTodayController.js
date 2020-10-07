import { getFormattedDatetimeString, replaceNullOrEmpty } from '/assets/js/util.js';
import { sortByUkprn } from '/assets/js/sortingUtils.js';
import JobReportControllerBase from './jobReportControllerBase.js';

class JobFailedTodayController extends JobReportControllerBase {

    constructor() {
        super('jobFailedTodayHub', sortByUkprn);
    }

    formatDataForDisplay() {
        this._data.jobs.map(p => {
            p.failedAtDateStr = getFormattedDatetimeString(p.failedAt),
            p.providerName = replaceNullOrEmpty(p.providerName, 'ESFA'),
            p.fileName = replaceNullOrEmpty(p.fileName, '')
        });
    }

    createReportRow(item) {
        return `<tr class="govuk-table__row">
                    <td class="govuk-table__cell" style="width:250px">${item.providerName}</td>
                    <td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>
                    <td class="govuk-table__cell" style="width:170px">${item.fileName}</td>
                    <td class="govuk-table__cell" style="width:170px">${item.failedAtDateStr}</td>
                    <td class="govuk-table__cell">${item.processingTimeBeforeFailure}</td>
                </tr>`
    }

    getCSVData() {
        const data = this._data.jobs.map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Filename": obj.fileName,
                "Failed at": obj.failedAtDateStr,
                "Processing time before failure": obj.processingTimeBeforeFailure
            }
        });

        return { data, fileName: 'Jobs-Failed-Today.csv' };
    }
}

export const jobFailedTodayController = new JobFailedTodayController()