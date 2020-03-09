import { convertToCsv } from '/assets/js/csv-operations.js';
import { getFormattedDatetimeString } from '/assets/js/util.js';
import { replaceNullOrEmpty } from '/assets/js/util.js';

class JobSlowFileController {

    constructor() {

        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');

        this._data = {};

    }

    updatePage(data) {

        this._data = typeof data === 'object' ? data : JSON.parse(data);

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
            sb.push(`<td class="govuk-table__cell" style="width:250px"><a href="#">${replaceNullOrEmpty(item.providerName, `ESFA`)}</a></td>`);
            sb.push(`<td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${replaceNullOrEmpty(item.fileName, ``)}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.timeTaken}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.averageTime}</td>`);
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
                    "Time taken": obj.timeTaken,
                    "Average time": obj.averageTime
                }
            });

            convertToCsv({ filename: 'Jobs-Slow-File.csv', data: newData });

        }

    }
}

export default JobSlowFileController