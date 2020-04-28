import { updateSync } from '/assets/js/periodEnd/baseController.js';
import { setControlEnabledState } from '/assets/js/util.js';
class jobController {

    constructor() {
        this._slowTimer = null;
    }

    renderJobs(state) {
        updateSync.call(this);

        if (state === "" || state === undefined) {
            return;
        }

        const stateModel = typeof state === 'object' ? state : JSON.parse(state);
        this.renderFailedJobs(stateModel.failedJobs);
    }

    renderFailedJobs(jobs) {
        let container = document.getElementById("failedJobContainer");
        this.clearContainer(container);

        const classScope = this;
        if (jobs != null && jobs.length > 0) {
            
            jobs.forEach(function(job) {
                let provider = job.ukprn + " - " + job.organisationName;

                let date = classScope.formatDate(job.dateTimeSubmitted);
                let jobDom =
                    `<div class="main-card">
				        <div class="flex govuk-!-margin-bottom-2">
					        <strong class="govuk-link govuk-!-font-size-19 provider-title ">${provider}</strong>
				        </div>
				        <div class="govuk-body card-wrapper ilr">
					        <div class="flex space-between govuk-!-margin-bottom-2">
						        <span class="wrong-file-name">${job.collectionName}</span>						       
					        </div>
					        <div class="flex space-between">
						        <span class="govuk-!-font-size-16">${job.fileName}</span>
						        <span class="govuk-!-font-size-16">${date}</span>
					        </div>
				        </div>
				        <hr class="main-divider">
			        </div>`;
                container.insertAdjacentHTML("beforeend", jobDom);
            });
        } else {
            let emptyDom = '<div class="govuk-!-font-size-16">No Failed Jobs</div>';
            container.insertAdjacentHTML("beforeend", emptyDom);
        }
    }

    clearContainer(container) {
        while (container.firstChild) {
            container.removeChild(container.firstChild);
        }
    }

    formatDate(dateString) {
        let dateOptions = { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric' };

        return new Date(dateString).toLocaleDateString("en-GB", dateOptions);
    }

    disableJobReSubmit(jobId) {
        setControlEnabledState(enabled, "jobButton" + jobId);
    }

    setCollectionClosedEmailButtonState(enabled) {
        setControlEnabledState(enabled, "collectionClosed");
    }

    setContinueButtonState(enabled) {
        setControlEnabledState(enabled, "startPeriodEnd");
    }

    setStartPeriodEndButtonState(enabled) {
        setControlEnabledState(enabled, "startPeriodEnd");
    }

    setClosePeriodEndButtonState(enabled) {
        setControlEnabledState(enabled, "closePeriodEnd");
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    padLeft (str, padString,  max) {
        str = str.toString();
        return str.length < max ? this.padLeft(padString + str, padString, max) : str;
    }
}

export default jobController;