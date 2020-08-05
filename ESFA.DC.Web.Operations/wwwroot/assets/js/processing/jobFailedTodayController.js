import { convertToCsv } from '/assets/js/csv-operations.js';
import { getFormattedDatetimeString } from '/assets/js/util.js';
import { replaceNullOrEmpty } from '/assets/js/util.js';

class JobFailedTodayController {

    constructor() {

        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');

        this._data = {};

    }

    updatePage(data) {

        if (typeof data === 'object') {
            this._data = data;
        }
        else {
            this._data = JSON.parse(data);
            this._data.jobs.map(p => {
                p.failedAtDateStr = getFormattedDatetimeString(p.failedAt),
                    p.providerName = replaceNullOrEmpty(p.providerName, 'ESFA'),
                    p.fileName = replaceNullOrEmpty(p.fileName, '')
            });
        }

        this.drawGrid();

    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
    }

    displayConnectionState(state) {

        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;

    }
    
    drawGrid() {

        this.sortByUkprn();

        this._aBtnDownloadCSV.style.visibility = (this._data.jobs.length > 0) ? 'visible' : 'hidden';

        var sb = [];

        for (var i = 0; i < this._data.jobs.length; i++) {

            var item = this._data.jobs[i];

            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell" style="width:250px">${item.providerName}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.fileName}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.failedAtDateStr}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.processingTimeBeforeFailure}</td>`);
            sb.push(`</tr>`);
        }

        var result = sb.join('');

        var dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = result;

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

export default JobFailedTodayController