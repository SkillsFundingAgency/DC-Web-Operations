import { getColorForPercentage } from '/assets/js/util.js';

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
        this._data = typeof data === 'object' ? state : JSON.parse(data);
        this.drawGrid();
    }

    setDonut() {
        this._firstDonut.setAttribute("data-count", this._data.jobs.length);
        this._firstDonut.textContent = this._data.jobs.length;

        let percentage = (this._data.jobs.length / 125) * 100;
        this._firstCircle.setAttribute("stroke-dasharray", `${percentage},100`);
        this._firstCircle.setAttribute("style", "stroke:" + getColorForPercentage(percentage));        
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    drawGrid() {
        if (this._data.jobCount !== undefined && this._data.jobCount !== null && this._data.jobCount > 0) {
            this.filterBy();
            this.setDonut();
            this.sortBy();

            var sb = [];
            for (var i = 0; i < this._data.jobs.length; i++) {
                var item = this._data.jobs[i];
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
    }

    sortBy() {
        if (this._sort) {
            switch (this._sort.value) {
                case 'LongestTimeInTheQueue':
                    this._data.jobs.sort(function (a, b) {
                        return a.timeInQueueSecond + b.timeInQueueSecond;
                    });
                    break;
                case 'ShortestTimeInTheQueue':
                    this._data.jobs.sort(function (a, b) {
                        return a.timeInQueueSecond - b.timeInQueueSecond;
                    });
                    break;
                case 'Alphabetical':
                    this._data.jobs.sort(function (a, b) {
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
                    this._data.jobs.sort(function (a, b) {
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
            var newArr = [];
            for (var i = 0; i < this._data.jobs.length; i++) {
                var item = this._data.jobs[i];
                var filter = filters.filter(x => x == item.collectionType);
                if (filter == item.collectionType) {
                    newArr.push(item);
                }
            }
            if (newArr.length > 0) {
                this._data.jobs = newArr;
            }
        }
    }

}

export default JobQueuedController