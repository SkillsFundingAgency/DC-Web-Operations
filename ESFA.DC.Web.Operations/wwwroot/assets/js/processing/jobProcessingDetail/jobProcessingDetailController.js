import { msToTime, getInitialStateModel, $on, replaceNullOrEmpty, setControlVisiblity} from '/assets/js/util.js';
import Client from '/assets/js/processing/jobProcessingDetail/client.js';
import JobReportControllerBase from '/assets/js/processing/jobReportControllerBase.js';

class JobProcessingDetailController extends JobReportControllerBase {

    constructor() {
        const state = getInitialStateModel();
        super({ hubUrl: 'jobProcessingDetailHub', initialState: { jobs: state.data } });
        this._jobProcessingType = state.jobProcessingType;
        this._hubClient = new Client(this._hub.getConnection());
    }

    formatDataForDisplay() {
        this._data.jobs.map(p => {
            p.providerName = replaceNullOrEmpty(p.providerName, 'ESFA'),
            p.fileName = replaceNullOrEmpty(p.fileName, '')
        });
    }

    registerEvents() {
        super.registerEvents();
        $on(document.getElementById("refreshData"), 'click', () => {
            this.refreshData();
        });
    }

    updatePage(data) {
        super.updatePage(data);
        setControlVisiblity(false, 'dataLoadingSpinner');
    }

    registerHandlers() { }

    refreshData() {
        setControlVisiblity(true, 'dataLoadingSpinner');
        if (this._jobProcessingType === "CurrentPeriod") {
            this._hubClient.getJobProcessingDetailsForCurrentPeriod(this.updatePage.bind(this));
        } else if (this._jobProcessingType === "LastHour") {
            this._hubClient.getJobProcessingDetailsForLastHour(this.updatePage.bind(this));
        } else if (this._jobProcessingType === "LastFiveMins") {
            this._hubClient.getJobProcessingDetailsForLastFiveMins(this.updatePage.bind(this));
        }
    }

    createReportRow(item) {
        return `<tr class="govuk-table__row">
                    <td class="govuk-table__cell" style="width:250px">${item.providerName}</td>
                    <td class="govuk-table__cell" style="width:100px">${item.ukprn}</td>
                    <td class="govuk-table__cell" style="width:170px">${item.fileName}</td>
                    <td class="govuk-table__cell" style="width:170px">${msToTime(item.processingTimeMilliSeconds)}</td>
                 </tr>`;
    }

    drawGrid() {
        super.drawGrid();
        if (this._data.jobs.length > 0) {
            paginator({
                table: document.getElementById("table_box_native").getElementsByTagName("table")[0],
                box: document.getElementById("index_native"),
                page: 1,
                rows_per_page: 25,
                page_options: false
            });
        }
    }

    getCSVData() {
        const data = this._data.jobs.map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Filename": obj.fileName,
                "Processing time ": obj.processingTimeMilliSeconds
            };
        });

        return { data, fileName: `JobsProcessingDetail-${this._jobProcessingType}.csv`};
    }
}

export const jobProcessingDetailController = new JobProcessingDetailController();