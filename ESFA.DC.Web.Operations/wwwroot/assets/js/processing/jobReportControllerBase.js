import { convertToCsv } from '/assets/js/csv-operations.js';
import { displayConnectionState, getInitialStateModel, $on, parseToObject } from '/assets/js/util.js';
import { sortByUkprn } from '/assets/js/sortingUtils.js';
import Hub from '/assets/js/hubs/hub.js';

class JobReportControllerBase {

    constructor({hubUrl, defaultSort = sortByUkprn, initialState = getInitialStateModel()} = {}) {
        this._defaultSortMethod = defaultSort;
        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');

        this._hub = new Hub(hubUrl, displayConnectionState);
        this.registerHandlers();
        this._hub.startHub();

        this.registerEvents();
        this.updatePage(initialState);
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
        this._data = parseToObject(data);
        this.formatDataForDisplay();
        this.drawGrid();
    }

    drawGrid() {
        this._data.jobs.sort(this._defaultSortMethod);
        this._aBtnDownloadCSV.style.visibility = (this._data.jobs.length > 0) ? 'visible' : 'hidden';

        let sb = [];
        this._data.jobs.forEach(item => sb.push(this.createReportRow(item)));

        const dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = sb.join('');
    }

    downloadCSV() {
        if (this._data.jobs.length > 0) {
            const csvData = this.getCSVData();
            convertToCsv({ filename: csvData.fileName, data: csvData.data });
        }
    }

    // Abstract methods
    formatDataForDisplay() { throw new Error('You have to implement the method formatDataForDisplay'); }
    createReportRow(item) { throw new Error('You have to implement the method createReportRow'); }
    getCSVData() { throw new Error('You have to implement the method getCSVData');}
}

export default JobReportControllerBase