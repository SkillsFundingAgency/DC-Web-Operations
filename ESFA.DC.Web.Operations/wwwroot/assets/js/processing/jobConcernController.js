import { convertToCsv } from '/assets/js/csv-operations.js';
import { getFormattedDatetimeString } from '/assets/js/util.js';
import { replaceNullOrEmpty } from '/assets/js/util.js';

class JobConcernController {

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
                    item.lastSuccessfulSubmission = getFormattedDatetimeString(item.lastSuccessfulSubmission)
            });
        }

        this.drawGrid();

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
            sb.push(`<td class="govuk-table__cell" style="width:250px"><a href="#">${item.providerName}</a></td>`);
            sb.push(`<td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.fileName}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.lastSuccessfulSubmission}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.periodOfLastSuccessfulSubmission}</td>`);
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
                    "Last successful submission": obj.lastSuccessfulSubmission,
                    "Period of last successful submission": obj.periodOfLastSuccessfulSubmission
                }
            });

            convertToCsv({ filename: 'Jobs-Concern.csv', data: newData });

        }

    }
}

export default JobConcernController