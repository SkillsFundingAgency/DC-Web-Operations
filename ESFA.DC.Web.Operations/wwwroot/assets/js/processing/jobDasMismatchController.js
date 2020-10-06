import { convertToCsv } from '/assets/js/csv-operations.js';
import { getFormattedDatetimeString, replaceNullOrEmpty, displayConnectionState, getInitialStateModel, $on, parseToObject } from '/assets/js/util.js';
import Hub from '/assets/js/hubs/hub.js';

class JobDasMismatchController {

    constructor() {
        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');

        const hub = new Hub('jobDasMismatchHub', displayConnectionState);
        this.registerHandlers(hub);
        hub.startHub();

        this.registerEvents();
        this.updatePage(getInitialStateModel());
    }

    updatePage(data) {
        this._data = parseToObject(data);
        this._data.jobs.map(item => {
            item.providerName = replaceNullOrEmpty(item.providerName, 'ESFA'),
                item.fileName = replaceNullOrEmpty(item.fileName, ''),
                item.submissionDate = getFormattedDatetimeString(item.submissionDate)
        });

        this.drawGrid();
    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
    }

    registerEvents() {
        $on(this._aBtnDownloadCSV, 'click', () => {
            this.downloadCSV();
        });
    }

    drawGrid() {
        this.sortByProviderName();
        this._aBtnDownloadCSV.style.visibility = (this._data.jobs.length > 0) ? 'visible' : 'hidden';
        const sb = [];

        for (var i = 0; i < this._data.jobs.length; i++) {
            const item = this._data.jobs[i];
            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell">${item.providerName}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.fileName}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.submissionDate}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.jobId}</td>`);
            sb.push(`</tr>`);
        }

        const dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = sb.join('');
    }

    sortByProviderName() {
        this._data.jobs.sort(function (a, b) {
            const nameA = a.providerName.toUpperCase();
            const nameB = b.providerName.toUpperCase();
            if (nameA < nameB) {
                return -1;
            }
            if (nameA > nameB) {
                return 1;
            }
            return 0;
        });
    }

    downloadCSV() {
        if (this._data.jobs.length > 0) {
            let newData = this._data.jobs.map(function (obj) {
                return {
                    "Provider name": obj.providerName,
                    "Ukprn": obj.ukprn,
                    "Filename": obj.fileName,
                    "Date/time of submission": obj.submissionDate,
                    "Job id": obj.jobId
                }
            });
            convertToCsv({ filename: 'Jobs-DasMismatch.csv', data: newData });
        }
    }
}

export const jobDasMismatchController = new JobDasMismatchController();