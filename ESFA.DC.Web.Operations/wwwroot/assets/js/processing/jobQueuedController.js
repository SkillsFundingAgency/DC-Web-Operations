﻿import { getColorForPercentage } from '/assets/js/util.js';

class JobQueuedController {
    constructor() {
        this._firstDonut = document.getElementById("firstDonut");
        this._firstCircle = document.getElementById("firstCircle");
        this._sort = document.getElementById('sort');
        this._ilr = document.getElementById('ILR');
        this._eas = document.getElementById('EAS');
        this._esf = document.getElementById('ESF');
        this._data = {};
    }

    updatePage(data) {
        this._data = typeof data === 'object' ? data : JSON.parse(data);
        this.drawGrid();
    }

    setDonut(filteredData) {
        this._firstDonut.setAttribute("data-count", filteredData.jobs.length);
        this._firstDonut.textContent = filteredData.jobs.length;

        let percentage = (filteredData.jobs.length / 125) * 100;
        this._firstCircle.setAttribute("stroke-dasharray", `${percentage},100`);
        this._firstCircle.setAttribute("style", "stroke:" + getColorForPercentage(percentage));
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    drawGrid() {
        var filteredData = this.filterBy();
        this.setDonut(filteredData);
        this.sortBy(filteredData);

        var sb = [];
        for (var i = 0; i < filteredData.jobs.length; i++) {
            var item = filteredData.jobs[i];
            sb.push(`<tr class="govuk-table__row">`);
            sb.push(`<td class="govuk-table__cell" style="width:500px"><a href="#">${item.providerName}</a></td>`);
            sb.push(`<td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>`);
            sb.push(`<td class="govuk-table__cell" style="width:170px">${item.timeInQueue}</td>`);
            sb.push(`</tr>`);
        }
        var result = sb.join('');

        var dataContent = document.getElementById("dataContent");
        dataContent.innerHTML = result;
    }

    sortBy(filteredData) {
        if (this._sort) {
            switch (this._sort.value) {
                case 'LongestTimeInTheQueue':
                    filteredData.jobs.sort(function (a, b) {
                        return a.timeInQueueSecond + b.timeInQueueSecond;
                    });
                    break;
                case 'ShortestTimeInTheQueue':
                    filteredData.jobs.sort(function (a, b) {
                        return a.timeInQueueSecond - b.timeInQueueSecond;
                    });
                    break;
                case 'Alphabetical':
                    filteredData.jobs.sort(function (a, b) {
                        var nameA = a.providerName.toUpperCase();
                        var nameB = b.providerName.toUpperCase();
                        if (nameA < nameB) {
                            return -1;
                        }
                        if (nameA > nameB) {
                            return 1;
                        }
                        return 0;
                    });
                    break;
                case 'Ukprn':
                    filteredData.jobs.sort(function (a, b) {
                        return a.ukprn - b.ukprn;
                    });
                    break;
            }
        }
    }

    filterBy() {
        var filters = [];
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

            var retVal = { jobCount: 0, jobs: [] }
            retVal.jobs = this._data.jobs.filter(function (array_el) {
                return filters.filter(function (anotherOne_el) {
                    return anotherOne_el == array_el.collectionType;
                }).length > 0
            });

            return retVal;
        }

        return this._data;
    }

}

export default JobQueuedController