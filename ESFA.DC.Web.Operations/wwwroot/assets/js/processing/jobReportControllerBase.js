import { convertToCsv } from '/assets/js/csv-operations.js';
import { displayConnectionState, getInitialStateModel, $on, parseToObject } from '/assets/js/util.js';
import { sortByUkprn } from '/assets/js/sortingUtils.js';
import { paginator } from '/assets/js/paginator.js';
import Hub from '/assets/js/hubs/hub.js';

class JobReportControllerBase {
    constructor({ hubUrl, defaultSort = sortByUkprn, initialState = getInitialStateModel() } = {}) {
        this._defaultSortMethod = defaultSort;
        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');
        this._pageNumber = 1;

        this._hub = new Hub(hubUrl, displayConnectionState);
        this.registerHandlers();
        this._hub.startHub();

        this.registerEvents();
        this.setCollectionYear(initialState.collectionYear);
        this.updatePage(initialState);
    }

    setCollectionYear(collectionYear) {
        this._collectionYear = collectionYear; this._collectionYearElement = document.getElementById('collectionYear');
        if (this._collectionYearElement !== null) {
            this._collectionYearElement.textContent = this._collectionYear;
        }
    }

    registerHandlers() {
        this._hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
    }

    registerEvents() {
        $on(this._aBtnDownloadCSV, 'click', () => {
            this.downloadCSV();
        });
    }

    updatePage(data) {
        this._data = { jobs: this.getJobs(parseToObject(data)) };
        this.formatDataForDisplay();
        if (this._data.jobs !== undefined) {
            this.drawGrid();
        }
    }

    getJobs(data) {
        if (this._collectionYear) {
            return data.jobs[this._collectionYear];
        }

        let jobs = [];
        for (const year in data.jobs) {
            jobs = jobs.concat(data.jobs[year]);
        }

        return jobs;
    }


    drawGrid() {
        this._data.jobs.sort(this._defaultSortMethod);
        this._aBtnDownloadCSV.style.visibility = (this._data.jobs.length > 0) ? 'visible' : 'hidden';

        let sb = [];
        this._data.jobs.forEach(item => sb.push(this.createReportRow(item)));

        const dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = sb.join('');

        if (this._data.jobs.length > 0) {
            paginator({
                table: document.getElementById("table_box_native").getElementsByTagName("table")[0],
                box: document.getElementById("index_native"),
                page: this._pageNumber,
                rows_per_page: 25,
                page_options: false,
                page_change_event: (config) => { this._pageNumber = config.page }
            });
        }
    }

    downloadCSV() {
        if (this._data.jobs !== undefined && this._data.jobs.length > 0) {
            const csvData = this.getCSVData();
            convertToCsv({ filename: csvData.fileName, data: csvData.data });
        }
    }

    // Abstract methods
    formatDataForDisplay() { throw new Error('You have to implement the method formatDataForDisplay'); }
    createReportRow(item) { throw new Error('You have to implement the method createReportRow'); }
    getCSVData() { throw new Error('You have to implement the method getCSVData'); }
}
export default JobReportControllerBase
