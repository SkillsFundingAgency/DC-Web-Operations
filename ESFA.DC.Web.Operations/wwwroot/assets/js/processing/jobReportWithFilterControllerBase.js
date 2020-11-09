import { convertToCsv } from '/assets/js/csv-operations.js';
import { displayConnectionState, getInitialStateModel, $on, $onAll, parseToObject, getColorForPercentage, getMessageForPercentage } from '/assets/js/util.js';
import { paginator } from '/assets/js/paginator.js';
import Hub from '/assets/js/hubs/hub.js';

class JobReportWithFilterControllerBase {

    constructor({ hubUrl, initialState = getInitialStateModel()}) {
        this._firstDonut = document.getElementById("firstDonut");
        this._firstCircle = document.getElementById("firstCircle");
        this._firstDonutText = document.getElementById("firstDonutText");
        this._pageNumber = 1;

        this._sort = document.getElementById('sort');
        this._ilr = document.getElementById('ILR');
        this._eas = document.getElementById('EAS');
        this._esf = document.getElementById('ESF');

        this._aBtnDownloadCSV = document.getElementById('aBtnDownloadCSV');
        this.registerEvents();

        this._percentageTextRange = [{ value: 85, label: 'Super Excited!'}, { value: 60, label: 'Needs Attention'}, { value: 0, label: 'Looking Good'}];

        this._hub = new Hub(hubUrl, displayConnectionState);
        this.registerHandlers(this._hub);
        this._hub.startHub();

        this.updatePage(initialState)
    }

    registerHandlers() {
        this._hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
    }

    registerEvents() {
        const dataRefreshElements = document.querySelectorAll("[name='sort'], [name='waste']");

        $onAll(dataRefreshElements, 'change', () => {
            this.updatePage(this._data);
        });

        $on(this._aBtnDownloadCSV, 'click', () => {
            this.downloadCSV();
        });
    }

    setDonut(filteredData) {
        this._firstDonut.setAttribute("data-count", filteredData.length);
        this._firstDonut.textContent = filteredData.length;

        const percentage = this.getPercentage(filteredData);
        this._firstCircle.setAttribute("stroke-dasharray", `${percentage}, 100`);
        this._firstCircle.setAttribute("style", "stroke:" + getColorForPercentage(percentage));
        this._firstDonutText.textContent = getMessageForPercentage(percentage, this._percentageTextRange);
    }

    updatePage(data) {
        this._data = { jobs: this.getJobs(parseToObject(data)) };
        this.formatDataForDisplay();
        this.drawGrid();
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
        const filteredData = this.filterBy();
        this._aBtnDownloadCSV.style.visibility = (filteredData.length > 0) ? 'visible' : 'hidden';
        this.setDonut(filteredData);
        this.sortBy(filteredData);

        let sb = [];
        filteredData.forEach(item => sb.push(this.createReportRow(item)));

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

    filterBy() {
        let filters = [];
        if (this._ilr && this._ilr.checked) {
            filters.push(this._ilr.value);
        }
        if (this._eas && this._eas.checked) {
            filters.push(this._eas.value);
        }
        if (this._esf && this._esf.checked) {
            filters.push(this._esf.value);
        }

        if (filters.length > 0) {
            return this._data.jobs.filter(function (array_el) {
                return filters.filter(function (anotherOne_el) {
                    return anotherOne_el === array_el.collectionType;
                }).length > 0
            });
        }

        return this._data.jobs;
    }

    downloadCSV() {
        const filteredData = this.filterBy();
        if (filteredData.length > 0) {
            const csvData = this.getCSVData(filteredData);
            convertToCsv({ filename: csvData.fileName, data: csvData.data });
        }
    }

    getPercentage(filteredData) {
        return (filteredData.length / 125) * 100;
    }

    // Abstract methods
    formatDataForDisplay() { throw new Error('You have to implement the method formatDataForDisplay'); }
    createReportRow(item) { throw new Error('You have to implement the method createReportRow'); }
    getCSVData() { throw new Error('You have to implement the method getCSVData'); }
    sortBy() { throw new Error('You have to implement the method sortBy'); }
  
}

export default JobReportWithFilterControllerBase