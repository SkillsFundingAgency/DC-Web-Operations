import { convertToCsv } from '/assets/js/csv-operations.js';
import { replaceNullOrEmpty, getFormattedDatetimeString, displayConnectionState, getInitialStateModel, $on, $onAll, parseToObject } from '/assets/js/util.js';
import Hub from '/assets/js/hubs/hub.js';

class JobFailedTodayController {

    constructor() {
        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');
        const hub = new Hub('jobFailedTodayHub', displayConnectionState);
        this.registerHandlers(hub);
        hub.startHub();

        this.registerEvents();
        this.updatePage(getInitialStateModel());
    }

    updatePage(data) {
        this._data = parseToObject(data);
        this._data.jobs.map(p => {
            p.failedAtDateStr = getFormattedDatetimeString(p.failedAt),
                p.providerName = replaceNullOrEmpty(p.providerName, 'ESFA'),
                p.fileName = replaceNullOrEmpty(p.fileName, '')
        });

        this.drawGrid();
    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
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

    drawGrid() {
        this.sortByUkprn();
        this._aBtnDownloadCSV.style.visibility = (this._data.jobs.length > 0) ? 'visible' : 'hidden';

        let sb = [];

        for (var i = 0; i < this._data.jobs.length; i++) {
            const item = this._data.jobs[i];
            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell" style="width:250px">${item.providerName}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.fileName}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.failedAtDateStr}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.processingTimeBeforeFailure}</td>`);
            sb.push(`</tr>`);
        }

        const dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = sb.join('');
    }

    sortByUkprn() {
        this._data.jobs.sort(function (a, b) {
            return a.ukprn - b.ukprn;
        });
    }

    downloadCSV() {
        if (this._data.jobs.length > 0) {
            let newData = this._data.jobs.map(function (obj) {
                return {
                    "Provider name": obj.providerName,
                    "Ukprn": obj.ukprn,
                    "Filename": obj.fileName,
                    "Failed at": obj.failedAtDateStr,
                    "Processing time before failure": obj.processingTimeBeforeFailure
                }
            });
            convertToCsv({ filename: 'Jobs-Failed-Today.csv', data: newData });
        }
    }
}

export const jobFailedTodayController = new JobFailedTodayController()