import { convertToCsv } from '/assets/js/csv-operations.js';
import { msToTime } from '/assets/js/util.js';

class JobProcessingDetailController {

    constructor() {
        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');
        this._data = {};
        this._refreshData = document.getElementById("refreshData");
        this._dataLoadingSpinner = document.getElementById('dataLoadingSpinner');
        this._jobProcessingType = null;
    }

    init(jobProcessingType) {
        this._jobProcessingType = jobProcessingType;
        this._refreshData.addEventListener("click", this.refreshData.bind(this));
    }

    refreshData() {
        this._dataLoadingSpinner.style.visibility = 'visible';
        if (this._jobProcessingType === "CurrentPeriod") {
            window.jobProcessingDetailClient.getJobProcessingDetailsForCurrentPeriod(this.updatePage.bind(this));
        } else if (this._jobProcessingType === "LastHour") {
            window.jobProcessingDetailClient.getJobProcessingDetailsForLastHour(this.updatePage.bind(this));
        } else if (this._jobProcessingType === "LastFiveMins") {
            window.jobProcessingDetailClient.getJobProcessingDetailsForLastFiveMins(this.updatePage.bind(this));
        }
    }

    updatePage(data) {
        if (typeof data === 'object') {
            this._data = data;
        }
        else {
            this._data = JSON.parse(data);
        }
        this.drawGrid();
        this._dataLoadingSpinner.style.visibility = 'hidden';
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    drawGrid() {
        this.sortByUkprn();
        this._aBtnDownloadCSV.style.visibility = (this._data.length > 0) ? 'visible' : 'hidden';
        var sb = [];
        for (var i = 0; i < this._data.length; i++) {
            var item = this._data[i];
            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell" style="width:250px"><a href="#">${item.providerName}</a></td>`);
            sb.push(`<td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.fileName}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${msToTime(item.processingTimeMilliSeconds)}</td>`);
            sb.push(`</tr>`);
        }

        var result = sb.join('');

        var dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = result;

        if (this._data.length > 0) {
            paginator({
                table: document.getElementById("table_box_native").getElementsByTagName("table")[0],
                box: document.getElementById("index_native"),
                page: 1,
                rows_per_page: 25,
                page_options: false
            });
        }
    }

    sortByUkprn() {
        this._data.sort(function (a, b) {
            return a.ukprn - b.ukprn;
        });
    }

    downloadCSV() {

        if (this._data.length > 0) {
            let newData = this._data.map(function (obj) {
                return {
                    "Provider name": obj.providerName,
                    "Ukprn": obj.ukprn,
                    "Filename": obj.fileName,
                    "Processing time ": obj.processingTimeMilliSeconds
                };
            });
            convertToCsv({ filename: 'JobsProcessingDetail-' + this._jobProcessingType + '.csv', data: newData });
        }
    }
}

export default JobProcessingDetailController;