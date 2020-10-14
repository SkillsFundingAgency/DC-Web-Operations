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
                    <td class="govuk-table__cell" style="width:250px">${item.providerName}</td>
                    <td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>
                    <td class="govuk-table__cell" style="width:170px">${item.fileName}</td>
                    <td class="govuk-table__cell" style="width:170px">${item.timeTaken}</td>
                    <td class="govuk-table__cell">${item.averageTime}</td>
                </tr>`;
    }

    getCSVData() {
        const data = this._data.jobs.map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Filename": obj.fileName,
                "Time taken": obj.timeTaken,
                "Average time": obj.averageTime
            };
        });

        return { data, fileName: 'Jobs-Slow-File.csv'};
    }
}

export const jobSlowFileController = new JobSlowFileController();
