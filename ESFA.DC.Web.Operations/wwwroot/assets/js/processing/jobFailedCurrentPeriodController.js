import { convertToCsv } from '/assets/js/csv-operations.js';
import { getFormattedDatetimeString } from '/assets/js/util.js';
import { getFormattedTimeString } from '/assets/js/util.js';
import { replaceNullOrEmpty } from '/assets/js/util.js';

class JobFailedCurrentPeriodController {

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

            this._data.jobs.map(item => {
                item.providerName = replaceNullOrEmpty(item.providerName, 'ESFA'),
                    item.fileName = replaceNullOrEmpty(item.fileName, ''),
                    item.dateTimeOfFailure = getFormattedDatetimeString(item.dateTimeOfFailure),
                    item.processingTimeBeforeFailure = getFormattedTimeString(item.processingTimeBeforeFailure)
            });
        }

        this.drawGrid();

    }

    displayConnectionState(state) {

        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;

    }

    drawGrid() {

        this.sortByProviderName();

        this._aBtnDownloadCSV.style.visibility = (this._data.jobs.length > 0) ? 'visible' : 'hidden';

        var sb = [];

        for (var i = 0; i < this._data.jobs.length; i++) {

            var item = this._data.jobs[i];

            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell"><a href="#">${item.providerName}</a></td>`);
            sb.push(`<td class="govuk-table__cell">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.fileName}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.dateTimeOfFailure}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.processingTimeBeforeFailure}</td>`);
            sb.push(`</tr>`);
        }

        var result = sb.join('');

        var dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = result;

    }

    sortByProviderName() {

        this._data.jobs.sort(function (a, b) {

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

export default JobFailedCurrentPeriodController