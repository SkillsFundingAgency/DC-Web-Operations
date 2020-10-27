import { getFormattedDatetimeString, replaceNullOrEmpty } from '/assets/js/util.js';
import JobReportControllerBase from './jobReportControllerBase.js';

class JobFailedTodayController extends JobReportControllerBase {

    constructor() {
        super({ hubUrl: 'jobFailedTodayHub' });
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
                    <td class="govuk-table__cell" style="width:200px">${item.providerName}</td>
                    <td class="govuk-table__cell" style="width:80px">${item.ukprn}</td>
                    <td class="govuk-table__cell" style="width:80px">${item.jobId}</td>
                    <td class="govuk-table__cell" style="width:160px">${item.fileName}</td>
                    <td class="govuk-table__cell" style="width:170px">${item.failedAtDateStr}</td>
                    <td class="govuk-table__cell">${item.processingTimeBeforeFailure}</td>
                </tr>`
    }

    getCSVData() {
        const data = this._data.jobs.filter(x => x.collectionType !== "PE").map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Job Id": obj.jobId,
                "Filename": obj.fileName,
                "Failed at": obj.failedAtDateStr,
                "Processing time before failure": obj.processingTimeBeforeFailure
            };
        });

        return { data, fileName: 'Jobs-Failed-Today.csv' };
    }
}

export const jobFailedTodayController = new JobFailedTodayController()