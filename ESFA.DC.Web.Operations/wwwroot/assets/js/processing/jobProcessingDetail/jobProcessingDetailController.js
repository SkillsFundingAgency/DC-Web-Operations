import { convertToCsv } from '/assets/js/csv-operations.js';
import { getFormattedDatetimeString } from '/assets/js/util.js';

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

    generatePagination(config) {
        const offset = 2;
        var current = config.page;
        var last = config.pages;
    const leftOffset = current - offset;
    const rightOffset = current + offset + 1;

    /**
     * Reduces a list into the page numbers desired in the pagination
     * @param {array} accumulator - Growing list of desired page numbers
     * @param {*} _ - Throwaway variable to ignore the current value in iteration
     * @param {*} idx - The index of the current iteration
     * @returns {array} The accumulating list of desired page numbers
     */
    function reduceToDesiredPageNumbers(accumulator, _, idx) {
        const currIdx = idx + 1;

        if (
            // Always include first page
            currIdx === 1
            // Always include last page
            || currIdx === last
            // Include if index is between the above defined offsets
            || (currIdx >= leftOffset && currIdx < rightOffset)) {
            return [
                ...accumulator,
                currIdx,
            ];
        }

        return accumulator;
    }

    /**
     * Transforms a list of desired pages and puts ellipsis in any gaps
     * @param {array} accumulator - The growing list of page numbers with ellipsis included
     * @param {number} currentPage - The current page in iteration
     * @param {number} currIdx - The current index
     * @param {array} src - The source array the function was called on
     */
    function transformToPagesWithEllipsis(accumulator, currentPage, currIdx, src) {
        const prev = src[currIdx - 1];

        // Ignore the first number, as we always want the first page
        // Include an ellipsis if there is a gap of more than one between numbers
        if (prev != null && currentPage - prev !== 1) {
            return [
                ...accumulator,
                '...',
                currentPage,
            ];
        }

        // If page does not meet above requirement, just add it to the list
        return [
            ...accumulator,
            currentPage,
        ];
    }

    const pageNumbers = Array(last)
        .fill()
        .reduce(reduceToDesiredPageNumbers, []);

    const pageNumbersWithEllipsis = pageNumbers.reduce(transformToPagesWithEllipsis, []);

    return pageNumbersWithEllipsis;
}

    pagination(config) {
        var current = config.pageNumber,
            last = 40,
            delta = 2,
            left = current - delta,
            right = current + delta + 1,
            range = [],
            rangeWithDots = [],
            l;

        for (let i = 1; i <= last; i++) {
            if (i == 1 || i == last || i >= left && i < right) {
                range.push(i);
            }
        }

        for (let i of range) {
            if (l) {
                if (i - l === 2) {
                    rangeWithDots.push(l + 1);
                } else if (i - l !== 1) {
                    rangeWithDots.push('...');
                }
            }
            rangeWithDots.push(i);
            l = i;
        }

        return rangeWithDots;
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
            sb.push(`<td class="govuk-table__cell">${item.processingTimeMilliSeconds}</td>`);
            sb.push(`</tr>`);
        }

        var result = sb.join('');

        var dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = result;

        paginator({
            table: document.getElementById("table_box_native").getElementsByTagName("table")[0],
            box: document.getElementById("index_native"),
            box_mode: this.generatePagination.bind(this),
            active_class: "color_page",
            page: 1,
            rows_per_page: 25,
            page_options: false
        });
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
            convertToCsv({ filename: 'JobsProcessingDetail-'+ this._jobProcessingType + '.csv', data: newData });
        }
    }
}

export default JobProcessingDetailController;