import { replaceNullOrEmpty } from '/assets/js/util.js';
import JobReportControllerBase from './jobReportControllerBase.js';

class JobSlowFileController extends JobReportControllerBase {

    constructor() {
        super({ hubUrl: 'jobSlowFileHub' });
    }

    formatDataForDisplay() {
        this._data.jobs.map(p => {
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
                    <td class="govuk-table__cell" style="width:170px">${item.timeTaken}</td>
                    <td class="govuk-table__cell">${item.averageTime}</td>
                </tr>`;
    }

    getCSVData() {
        const data = this._data.jobs.filter(x => x.collectionType !== "PE").map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Job Id": obj.jobId,
                "Filename": obj.fileName,
                "Time taken": obj.timeTaken,
                "Average time": obj.averageTime
            };
        });

        return { data, fileName: 'Jobs-Slow-File.csv'};
    }
}

export const jobSlowFileController = new JobSlowFileController();
