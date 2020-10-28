import { replaceNullOrEmpty } from '/assets/js/util.js';
import { sortByUkprn, sortByProviderName } from '/assets/js/sortingUtils.js';
import JobReportWithFilterControllerBase from './jobReportWithFilterControllerBase.js';

class JobQueuedController extends JobReportWithFilterControllerBase {

    constructor() {
        super({ hubUrl: 'jobQueuedHub' });
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
                    <td class="govuk-table__cell" style="width:160px">${item.timeInQueue}</td>
                    <td class="govuk-table__cell">${item.statusDescription}</td>
                </tr>`
    }

    sortBy(filteredData) {
        if (this._sort) {
            switch (this._sort.value) {
                case 'LongestTimeInTheQueue':
                    filteredData.sort(function (a, b) {
                        return b.timeInQueueSecond - a.timeInQueueSecond;
                    });
                    break;
                case 'ShortestTimeInTheQueue':
                    filteredData.sort(function (a, b) {
                        return a.timeInQueueSecond - b.timeInQueueSecond;
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
                "Collection type": obj.collectionType,
                "Job status": obj.statusDescription,
                "Time in queue": obj.timeInQueue
            };
        });
        return { data: csvData, fileName: 'Jobs-queued.csv' };
    }
}

export const jobQueuedController = new JobQueuedController();