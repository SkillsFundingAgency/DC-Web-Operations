import { getColorForPercentage, replaceNullOrEmpty, getMessageForPercentage, displayConnectionState, $on, $onAll, parseToObject, getInitialStateModel} from '/assets/js/util.js';
import { convertToCsv } from '/assets/js/csv-operations.js';
import Hub from '/assets/js/hubs/hub.js';

class JobQueuedController {
    constructor() {
        this._firstDonut = document.getElementById("firstDonut");
        this._firstCircle = document.getElementById("firstCircle");
        this._firstDonutText = document.getElementById("firstDonutText");

        this._sort = document.getElementById('sort');
        this._ilr = document.getElementById('ILR');
        this._eas = document.getElementById('EAS');
        this._esf = document.getElementById('ESF');

        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');
        this.registerEvents();

        this._percentageTextRangeJobQueued = [{ value: 85, label: 'Urgent Attention!' }, { value: 60, label: 'Needs Attention' }, { value: 0, label: 'Looking Good' }];
       
        const hub = new Hub('jobQueuedHub', displayConnectionState);
        this.registerHandlers(hub);
        hub.startHub();
        this.updatePage(getInitialStateModel())
    }

    updatePage(data) {
        this._data = parseToObject(data);

        this._data.jobs.map(p => {
            p.providerName = replaceNullOrEmpty(p.providerName, 'ESFA')
        });

        this.drawGrid();
    }

    registerEvents() {
        const dataRefreshElements = document.querySelectorAll("[name='sort'], [name='waste']");

        $onAll(dataRefreshElements, 'change', () => {
            this.updatePage(this._data);
        });

        $on(this._aBtnDownloadCSV, 'click', () => {
            this.downloadCSV();
        });
    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
    }

    setDonut(filteredData) {
        this._firstDonut.setAttribute("data-count", filteredData.length);
        this._firstDonut.textContent = filteredData.length;

        const percentage = (filteredData.length / 125) * 100;
        this._firstCircle.setAttribute("stroke-dasharray", `${percentage},100`);
        this._firstCircle.setAttribute("style", "stroke:" + getColorForPercentage(percentage));
        this._firstDonutText.textContent = getMessageForPercentage(percentage, this._percentageTextRangeJobQueued);
    }

    drawGrid() {
        const filteredData = this.filterBy();

        this._aBtnDownloadCSV.style.visibility = (filteredData.length > 0) ? 'visible' : 'hidden';
        this.setDonut(filteredData);
        this.sortBy(filteredData);

        let sb = [];
        for (var i = 0; i < filteredData.length; i++) {
            const item = filteredData[i];
            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell" style="width:420px">${item.providerName}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.timeInQueue}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.statusDescription}</td>`);
            sb.push(`</tr>`);
        }

        const dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = sb.join('');;
    }

    filterBy() {
        let filters = [];
        if (this._ilr && this._ilr.checked) {
            filters.push(this._ilr.value);
        }
        if (this._eas && this._eas.checked) {
            filters.push(this._eas.value);
        }
        if (this._esf && this._esf.checked) {
            filters.push(this._esf.value);
        }

        if (filters.length > 0) {
            return this._data.jobs.filter(function (array_el) {
                return filters.filter(function (anotherOne_el) {
                    return anotherOne_el == array_el.collectionType;
                }).length > 0
            });
        }

        return this._data.jobs;
    }

    sortBy(filteredData) {
        if (this._sort) {
            switch (this._sort.value) {
                case 'LongestTimeInTheQueue':
                    filteredData.sort(function (a, b) {
                        return a.timeInQueueSecond + b.timeInQueueSecond;
                    });
                    break;
                case 'ShortestTimeInTheQueue':
                    filteredData.sort(function (a, b) {
                        return a.timeInQueueSecond - b.timeInQueueSecond;
                    });
                    break;
                case 'Alphabetical':
                    filteredData.sort(function (a, b) {
                        var nameA = a.providerName.toUpperCase();
                        var nameB = b.providerName.toUpperCase();
                        if (nameA < nameB) {
                            return -1;
                        }
                        if (nameA > nameB) {
                            return 1;
                        }
                        return 0;
                    });
                    break;
                case 'Ukprn':
                    filteredData.sort(function (a, b) {
                        return a.ukprn - b.ukprn;
                    });
                    break;
            }
        }
    }

    downloadCSV() {
        const filteredData = this.filterBy();

        if (filteredData.length > 0) {
            let newData = filteredData.map(function (obj) {
                return {
                    "Provider name": obj.providerName,
                    Ukprn: obj.ukprn,
                    "Collection type": obj.collectionType,
                    "Job status": obj.statusDescription,
                    "Time in queue": obj.timeInQueue
                }
            });
            convertToCsv({ filename: 'Jobs-queued.csv', data: newData });
        }
    }
}

export const jobQueuedController = new JobQueuedController();