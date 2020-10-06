import { convertToCsv } from '/assets/js/csv-operations.js';
import { msToTime, displayConnectionState, getInitialStateModel, $on, parseToObject} from '/assets/js/util.js';
import Hub from '/assets/js/hubs/hub.js';
import Client from '/assets/js/processing/jobProcessingDetail/client.js';

class JobProcessingDetailController {

    constructor() {
        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');
        this._refreshData = document.getElementById("refreshData");
        this._dataLoadingSpinner = document.getElementById('dataLoadingSpinner');

        const hub = new Hub('jobProcessingDetailHub', displayConnectionState);
        hub.startHub();

        this._hubClient = new Client(hub.getConnection());
        this.registerEvents();

        const state = getInitialStateModel();
        this._jobProcessingType = state.jobProcessingType;
        this.updatePage(state.data);
    }

    registerEvents() {
        $on(this._aBtnDownloadCSV, 'click', () => {
            this.downloadCSV();
        });

        $on(this._refreshData, 'click', () => {
            this.refreshData();
        });
    }

    refreshData() {
        this._dataLoadingSpinner.style.visibility = 'visible';
        if (this._jobProcessingType === "CurrentPeriod") {
            this._hubClient.getJobProcessingDetailsForCurrentPeriod(this.updatePage.bind(this));
        } else if (this._jobProcessingType === "LastHour") {
            this._hubClient.getJobProcessingDetailsForLastHour(this.updatePage.bind(this));
        } else if (this._jobProcessingType === "LastFiveMins") {
            this._hubClient.getJobProcessingDetailsForLastFiveMins(this.updatePage.bind(this));
        }
    }

    updatePage(data) {
        this._data = parseToObject(data);
        this.drawGrid();
        this._dataLoadingSpinner.style.visibility = 'hidden';
    }

    drawGrid() {
        this.sortByUkprn();
        this._aBtnDownloadCSV.style.visibility = (this._data.length > 0) ? 'visible' : 'hidden';
        let sb = [];
        for (var i = 0; i < this._data.length; i++) {
            var item = this._data[i];
            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell" style="width:250px">${item.providerName}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.fileName}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${msToTime(item.processingTimeMilliSeconds)}</td>`);
            sb.push(`</tr>`);
        }

        const dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = sb.join('');;

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

export const jobProcessingDetailController = new JobProcessingDetailController();