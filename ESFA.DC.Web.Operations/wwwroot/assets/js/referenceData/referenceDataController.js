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
                    <td class="govuk-table__cell govuk-!-font-weight-bold">${file.displayDate}</td>
                    <td class="govuk-table__cell">${file.submittedBy}</td>
                    <td class="govuk-table__cell"><a href="/referenceData/campusIdentifiers/getReportFile/${file.fileName}">${fileName}</a></td>
                    <td class="govuk-table__cell">${file.jobId}</td>
                    <td class="govuk-table__cell">
                        <span>${file.displayStatus}</span> <br />
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