import { convertToCsv } from '/assets/js/csv-operations.js';
import { getFormattedDatetimeString, getFormattedTimeString, replaceNullOrEmpty, getInitialStateModel, displayConnectionState, $on, parseToObject} from '/assets/js/util.js';
import Hub from '/assets/js/hubs/Hub.js';

class JobFailedCurrentPeriodController {

    constructor() {
        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');
        const hub = new Hub('jobFailedCurrentPeriodHub', displayConnectionState);
        this.registerHandlers(hub);
        hub.startHub();
        this.registerEvents();
        this.updatePage(getInitialStateModel());
    }

    registerEvents() {
        $on(this._aBtnDownloadCSV, 'click', () => {
            this.downloadCSV();
        });
    }

    updatePage(data) {
        this._data = parseToObject(data);
        this._data.jobs.map(item => {
            item.providerName = replaceNullOrEmpty(item.providerName, 'ESFA'),
                item.fileName = replaceNullOrEmpty(item.fileName, ''),
                item.dateTimeOfFailure = getFormattedDatetimeString(item.dateTimeOfFailure),
                item.processingTimeBeforeFailure = getFormattedTimeString(item.processingTimeBeforeFailure)
        });
        this.drawGrid();
    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
    }

    drawGrid() {
        this.sortByProviderName();
        this._aBtnDownloadCSV.style.visibility = (this._data.jobs.length > 0) ? 'visible' : 'hidden';

        let sb = [];
        for (var i = 0; i < this._data.jobs.length; i++) {
            const item = this._data.jobs[i];
            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell">${item.providerName}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.fileName}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.dateTimeOfFailure}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.processingTimeBeforeFailure}</td>`);
            sb.push(`</tr>`);
        }

        const dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = sb.join('');;
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
                    "Date/time of failure": obj.dateTimeOfFailure,
                    "Processing time before failure": obj.processingTimeBeforeFailure
                }
            });

            convertToCsv({ filename: 'Jobs-FailedInCurrentPeriod.csv', data: newData });
        }
    }
}

export const jobFailedCurrentPeriodController = new JobFailedCurrentPeriodController();