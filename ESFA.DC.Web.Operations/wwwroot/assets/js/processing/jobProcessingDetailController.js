import { convertToCsv } from '/assets/js/csv-operations.js';
import { getFormattedDatetimeString } from '/assets/js/util.js';

class JobProcessingDetailController {

    constructor() {

        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');
        this._data = {};
        this._pageNumber = 1;
    }

    updatePage(data) {
        if (typeof data === 'object') {
            this._data = data;
        }
        else {
            this._data = JSON.parse(data);
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
            sb.push(`<td class="govuk-table__cell" style="width:100px">${item.ukPrn}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.filename}</td>`);
            sb.push(`<td class="govuk-table__cell">${item.processingTime}</td>`);
            sb.push(`</tr>`);
        }

        var result = sb.join('');

        var dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = result;

        paginator({
            table: document.getElementById("table_box_native").getElementsByTagName("table")[0],
            box: document.getElementById("index_native"),
            active_class: "color_page",
            page: this._pageNumber,
            rows_per_page: 5,
            page_options:false,
            tail_call: this.onTableRender.bind(this)
    });
    }

    onTableRender(config) {
        this._pageNumber = config.page;
    }
    sortByUkprn() {
        this._data.jobs.sort(function (a, b) {
            return a.ukprn - b.ukprn;
        });}

    downloadCSV() {

        if (this._data.jobs.length > 0) {
            let newData = this._data.jobs.map(function (obj) {
                return {
                    "Provider name": obj.providerName,
                    "Ukprn": obj.ukPrn,
                    "Filename": obj.filename,
                    "Processing time ": obj.processingTime
                };
            });
            convertToCsv({ filename: 'Jobs-Failed-Today.csv', data: newData });
        }
    }
}

export default JobProcessingDetailController;