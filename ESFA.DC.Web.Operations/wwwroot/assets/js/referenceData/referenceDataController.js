import { updateSync } from '/assets/js/periodEnd/baseController.js';

class referenceDataController {

    constructor() {
        this._slowTimer = null;
    }
    
    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    sortByDate(stateModel) {

        stateModel.files.sort(function (a, b) {
            return Number(a.submissionDate) - Number(b.submissionDate);
        });
    }

    renderFiles(controllerName, state) {
        updateSync.call(this);

        if (state === "" || state === undefined) {
            return;
        }

        const stateModel = typeof state === 'object' ? state : JSON.parse(state);
        if (!stateModel || !stateModel.files) {
            return;
        }

        this.sortByDate(stateModel);

        const fileContainer = document.getElementById('fileContainer');

        let updatedContent = '';

        stateModel.files.forEach(function(file) {
            const reportName = file.reportName ? file.reportName : '';
            const statusClass = file.displayStatus === 'Job Completed' ? 'jobCompleted' 
                : file.displayStatus === 'Job Rejected' ? 'jobRejected'
                : file.displayStatus === 'Job Failed' ? 'jobFailed'
                : '';

            const content = 
                `<tr class="govuk-table__row">
                    <td class="govuk-table__cell govuk-!-font-weight-bold" govuk-!-width-one-quarter>${file.displayDate}</td>
                    <td class="govuk-table__cell">${file.submittedBy}</td>
                    <td class="govuk-table__cell"><a href="/referenceData/${controllerName}/getReportFile/${file.collectionName}/${file.fileName}">${file.fileName}</a></td>
                    <td class="govuk-table__cell">${file.jobId}</td>
                    <td class="govuk-table__cell">
                        <span class="${statusClass}">${file.displayStatus}</span> <br />
                        <span class="govuk-!-font-weight-bold">${file.recordCount} records</span> <br />
                        <span class="govuk-!-font-weight-bold">${file.warningCount} warnings</span> <br />
                        <span class="govuk-!-font-weight-bold">${file.errorCount} errors</span> <br />
                    </td>
                    <td class="govuk-table__cell"><a href="/referenceData/${controllerName}/getReportFile/${file.collectionName}/${file.reportName}/${file.jobId}">${reportName}</a></td>
                </tr>`;
            updatedContent += content;
        });

        fileContainer.innerHTML = updatedContent;
    }
}

export default referenceDataController;