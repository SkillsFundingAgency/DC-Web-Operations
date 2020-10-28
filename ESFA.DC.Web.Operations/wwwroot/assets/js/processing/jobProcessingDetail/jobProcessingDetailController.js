import { msToTime, getInitialStateModel, $on, replaceNullOrEmpty, setControlVisiblity} from '/assets/js/util.js';
import Client from '/assets/js/processing/jobProcessingDetail/client.js';
import JobReportControllerBase from '/assets/js/processing/jobReportControllerBase.js';

class JobProcessingDetailController extends JobReportControllerBase {

    constructor() {
        const state = getInitialStateModel();
        super({ hubUrl: 'jobProcessingDetailHub' });
        this._jobProcessingType = state.jobProcessingType;
        this._hubClient = new Client(this._hub.getConnection());
    }

    formatDataForDisplay() {
        if (this._data.jobs !== undefined) {
            this._data.jobs.map(p => {
                p.providerName = replaceNullOrEmpty(p.providerName, 'ESFA'),
                    p.fileName = replaceNullOrEmpty(p.fileName, '')
            });
        }
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
                    <td class="govuk-table__cell" style="width:200px">${item.providerName}</td>
                    <td class="govuk-table__cell" style="width:80px">${item.ukprn}</td>
                    <td class="govuk-table__cell" style="width:80px">${item.jobId}</td>
                    <td class="govuk-table__cell" style="width:160px">${item.fileName}</td>
                    <td class="govuk-table__cell" style="width:170px">${msToTime(item.processingTimeMilliSeconds)}</td>
                 </tr>`;
    }

    getCSVData() {
        const data = this._data.jobs.filter(x => x.collectionType !== "PE").map(function (obj) {
            return {
                "Provider name": obj.providerName,
                "Ukprn": obj.ukprn,
                "Job Id": obj.jobId,
                "Filename": obj.fileName,
                "Processing time ": obj.processingTimeMilliSeconds
            };
        });

        return { data, fileName: `JobsProcessingDetail-${this._jobProcessingType}.csv`};
    }
}

export const jobProcessingDetailController = new JobProcessingDetailController();