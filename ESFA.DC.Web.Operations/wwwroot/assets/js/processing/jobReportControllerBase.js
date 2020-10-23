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
        this.setCollectionYear(document.getElementById('initialState').innerHTML);
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
        if (this._data.jobs?.jobs !== undefined) {
            this._data.jobs = this._data.jobs.jobs;
        }
        if (this._collectionYear === null) {
            if (this._data.jobs != undefined) {
                for (const [key, value] of Object.entries(this._data.jobs)) {
                    var ret = [];
                    ret.push(value.flat());
                }
                this._data.jobs = ret.flat();
            }
        } else {
            this._data.jobs = this._data.jobs[this._collectionYear];
        }

        if (this._data.jobs === undefined) {
            this._data.jobs = [];
        }

        this.formatDataForDisplay();
        this.drawGrid();

        this._collectionYearElement = document.getElementById('collectionYear');
        if (this._collectionYearElement !== null) {
            this._collectionYearElement.textContent = this._collectionYear;
        }
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

    setCollectionYear(data) {
        this._data = parseToObject(data);
        this._collectionYear = this._data.collectionYear;
    }

    // Abstract methods
    formatDataForDisplay() { throw new Error('You have to implement the method formatDataForDisplay'); }
    createReportRow(item) { throw new Error('You have to implement the method createReportRow'); }
    getCSVData() { throw new Error('You have to implement the method getCSVData');}
}

export default JobReportControllerBase