import { replaceNullOrEmpty} from '/assets/js/util.js';
import { sortByUkprn, sortByProviderName } from '/assets/js/sortingUtils.js';
import JobReportWithFilterControllerBase from './jobReportWithFilterControllerBase.js';

class JobProcessingController extends JobReportWithFilterControllerBase {

    constructor() {
        super({ hubUrl: 'jobProcessingHub'});
    }

    formatDataForDisplay() {
        this._data.jobs.map(p => {
            p.providerName = replaceNullOrEmpty(p.providerName, 'ESFA')
        });
    }

    createReportRow(item) {
        return `<tr class="govuk-table__row">
                    <td class="govuk-table__cell" style="width:350px">${item.providerName}</td>
                    <td class="govuk-table__cell" style="width:80px">${item.ukprn}</td>
                    <td class="govuk-table__cell" style="width:80px">${item.jobId}</td>
                    <td class="govuk-table__cell" style="width:160px">${item.timeTaken}</td>
                    <td class="govuk-table__cell">${item.averageProcessingTime}</td>
                </tr>`;
    }

    sortBy(filteredData) {
        if (this._sort) {
            switch (this._sort.value) {
                case 'TimeTaken':
                    filteredData.sort(function (a, b) {
                        return b.timeTakenSecond - a.timeTakenSecond;
                    });
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
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Job Id": obj.jobId,
                "Time taken": obj.timeTaken,
                "Average processing time": obj.averageProcessingTime
            };
        });
        return { data: csvData, fileName: 'Jobs-processing.csv' };
    }
}

export const jobProcessingController = new JobProcessingController();