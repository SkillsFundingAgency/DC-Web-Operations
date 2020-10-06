import { convertToCsv } from '/assets/js/csv-operations.js';
import { getFormattedDatetimeString, replaceNullOrEmpty, displayConnectionState, getInitialStateModel, $on, parseToObject } from '/assets/js/util.js';
import Hub from '/assets/js/hubs/hub.js';

class JobConcernController {

    constructor() {
        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');

        const hub = new Hub('jobConcernHub', displayConnectionState);
        this.registerHandlers(hub);
        hub.startHub();

        this.registerEvents();
        this.updatePage(getInitialStateModel());
    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
    }

    registerEvents() {
        $on(this._aBtnDownloadCSV, 'click', () => {
            this.downloadCSV();
        });
    }

    updatePage(data) {
        this._data = parseToObject(data);
        console.log(data);

        this._data.jobs.map(item => {
            item.providerName = replaceNullOrEmpty(item.providerName, 'ESFA'),
                item.fileName = replaceNullOrEmpty(item.fileName, ''),
                item.lastSuccessfulSubmission = getFormattedDatetimeString(item.lastSuccessfulSubmission)
        });
        this.drawGrid();
    }

    drawGrid() {
        this.sortByUkprn();
        this._aBtnDownloadCSV.style.visibility = (this._data.jobs.length > 0) ? 'visible' : 'hidden';
        let sb = [];

        for (var i = 0; i < this._data.jobs.length; i++) {
            const item = this._data.jobs[i];
            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell">${item.providerName}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.fileName}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.lastSuccessfulSubmission}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.periodOfLastSuccessfulSubmission}</td>`);
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
                    "Last successful submission": obj.lastSuccessfulSubmission,
                    "Period of last successful submission": obj.periodOfLastSuccessfulSubmission
                }
            });
            convertToCsv({ filename: 'Jobs-Concern.csv', data: newData });
        }
    }
}

export const jobConcernController = new JobConcernController();