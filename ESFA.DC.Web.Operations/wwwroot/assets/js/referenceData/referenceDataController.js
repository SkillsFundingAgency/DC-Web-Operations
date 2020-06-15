import { jobStatus } from '/assets/js/periodEnd/state.js';
import { updateSync } from '/assets/js/periodEnd/baseController.js';
class referenceDataController {

    constructor() {
        this._slowTimer = null;
        this._year = 0;
        this._period = 0;
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    jobStatusConvertor(status) {
        switch (status) {
        case jobStatus.ready:
            return "Ready";
        case jobStatus.movedForProcessing:
            return "Moved For Processing";
        case jobStatus.processing:
            return "Processing";
        case jobStatus.completed:
            return "Completed";
        case jobStatus.failedRetry:
            return "Failed Retry";
        case jobStatus.failed:
            return "Failed";
        case jobStatus.paused:
            return "Paused";
        case jobStatus.waiting:
            return "Waiting";
        default:
            return "";
        }
    }

    renderFiles(state) {
        updateSync.call(this);

        if (state === "" || state === undefined) {
            return;
        }

        const stateModel = typeof state === 'object' ? state : JSON.parse(state);

        let fileContainer = document.getElementById('fileContainer');
        let classScope = this;
        let updatedContent = '';
        stateModel.files.forEach(function(file) {
            var fileName = file.fileName ? file.fileName : '';
            var reportName = file.reportName ? file.reportName : '';
            const content = 
                `<tr class="govuk-table__row">
                    <td class="govuk-table__cell govuk-!-font-weight-bold">${file.submissionDate}</td>
                    <td class="govuk-table__cell">${file.submittedBy}</td>
                    <td class="govuk-table__cell"><a href="/referenceData/campusIdentifiers/getReportFile/${file.fileName}">${fileName}</a></td>
                    <td class="govuk-table__cell">${file.periodNumber}</td>
                    <td class="govuk-table__cell">
                        <span>${classScope.jobStatusConvertor(file.jobStatus)}</span> <br />
                        <span class="govuk-!-font-weight-bold">${file.recordCount} records</span> <br />
                        <span class="govuk-!-font-weight-bold">${file.errorCount} errors</span> <br />
                    </td>
                    <td class="govuk-table__cell"><a href="/referenceData/campusIdentifiers/getReportFile/${file.reportName}/${stateModel.period}/${file.jobId}">${reportName}</a></td>
                </tr>`;
            updatedContent += content;
        });

        fileContainer.innerHTML = updatedContent;
    }
}

export default referenceDataController;