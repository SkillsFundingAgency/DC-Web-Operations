import { convertToCsv } from '/assets/js/csv-operations.js';
import { replaceNullOrEmpty, getFormattedTimeString, getFormattedDatetimeString, displayConnectionState, $on, parseToObject, getInitialStateModel} from '/assets/js/util.js';
import Hub from '/assets/js/hubs/hub.js';

class JobProvidersReturnedCurrentPeriodController {

    constructor() {
        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');
        this.registerEvents();

        const hub = new Hub('jobProvidersReturnedCurrentPeriodHub', displayConnectionState);
        this.registerHandlers(hub);
        hub.startHub();
        this.updatePage(getInitialStateModel())
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
                item.dateTimeSubmission = getFormattedDatetimeString(item.dateTimeSubmission),
                item.processingTime = getFormattedTimeString(item.processingTime)
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
            sb.push(`<td class="govuk-table__cell">${item.dateTimeSubmission}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.processingTime}</td>`);
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
                    "Date/time of submission (latest)": obj.dateTimeSubmission,
                    "Processing time": obj.processingTime
                }
            });
            convertToCsv({ filename: 'Jobs-ProvidersReturnedCurrentPeriod.csv', data: newData });
        }
    }
}

export const jobProvidersReturnedCurrentPeriodController = new JobProvidersReturnedCurrentPeriodController()