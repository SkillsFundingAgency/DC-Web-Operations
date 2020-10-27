import { replaceNullOrEmpty, getDatetimeFromString, getFormattedDatetimeString } from '/assets/js/util.js';
import { sortByUkprn, sortByProviderName, sortByDateTime } from '/assets/js/sortingUtils.js';
import JobReportWithFilterControllerBase from './jobReportWithFilterControllerBase.js';

class JobSubmittedController extends JobReportWithFilterControllerBase {

    constructor() {
        super({ hubUrl: 'jobSubmittedHub' });
    }

    formatDataForDisplay() {
        this._data.jobs.map(p => {
            p.providerName = replaceNullOrEmpty(p.providerName),
            p.fileName = replaceNullOrEmpty(p.fileName),
            p.datetime = getDatetimeFromString(p.createdDate),
            p.createdDateStr = getFormattedDatetimeString(p.createdDate)
        });
    }

    createReportRow(item) {
        return `<tr class="govuk-table__row">
                    <td class="govuk-table__cell" style="width:200px">${replaceNullOrEmpty(item.providerName, '')}</td>
                    <td class="govuk-table__cell" style="width:80px">${item.ukprn}</td>
                    <td class="govuk-table__cell" style="width:80px">${item.jobId}</td>
                    <td class="govuk-table__cell" style="width:190px">${item.createdDateStr}</td>
                    <td class="govuk-table__cell" style="width:170px">${replaceNullOrEmpty(item.fileName, '')}</td>
                    <td class="govuk-table__cell">${item.statusDescription}</td>
                </tr>`;
    }

    sortBy(filteredData) {
        if (this._sort) {
            switch (this._sort.value) {
                case 'Datetime':
                    filteredData.sort(sortByDateTime);
                    break;
                case 'Alphabetical':
                    filteredData.sort(sortByProviderName);
                    break;
                case 'Ukprn':
                    filteredData.sort(sortByUkprn);
                    break;
            }
        }
    }

    getCSVData(data) {
        const csvData = data.filter(x => x.collectionType !== "PE").map(function (obj) {
            return {
                "Provider name": replaceNullOrEmpty(obj.providerName, ''),
                "Ukprn": obj.ukprn,
                "Job Id": obj.jobId,
                "Date/time": obj.createdDateStr,
                "Filename": replaceNullOrEmpty(obj.fileName, ''),
                "Job status": obj.statusDescription
            }
        });
        return { data: csvData, fileName: 'Jobs-Submitted.csv' };
    }

    getPercentage(filteredData) {
        if (filteredData.length < 2500) {
            return (filteredData.length / 2500) * 100;
        }
        return 100;
    }
}

export const jobSubmittedController = new JobSubmittedController()