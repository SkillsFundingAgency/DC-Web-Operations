﻿import { updateSync } from '/assets/js/baseController.js';
import { setControlEnabledState } from '/assets/js/util.js';
class jobController {

    constructor() {
        this._slowTimer = null;
        this._mcaLabel = document.getElementById("mcaLabel");
        this._sldDasLabel = document.getElementById("sldDasMismatch");
    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (state) => this.renderJobs(state));
        hub.registerMessageHandler("DisableJobReSubmit", (jobId) => this.disableJobReSubmit(jobId));
        hub.registerMessageHandler("ReferenceJobsButtonState", (enabled) => this.setPauseRefJobsButtonState(enabled));
        hub.registerMessageHandler("CollectionClosedEmailButtonState", (enabled) => this.setCollectionClosedEmailButtonState(enabled));
        hub.registerMessageHandler("ContinueButtonState", (enabled) => this.setContinueButtonState(enabled));
    }

    renderJobs(state) {
        updateSync.call(this);

        if (state === "" || state === undefined) {
            return;
        }

        const stateModel = typeof state === 'object' ? state : JSON.parse(state);
        this.renderMcas(stateModel.mcaDetails);
        this.renderSLDDASMismatch(stateModel.slddasMismatches);
        this.renderFailedJobs(stateModel.failedJobs);
        this.renderReferenceJobs(stateModel.referenceDataJobs);
    }

    renderReferenceJobs(jobs) {
        let container = document.getElementById("referenceDataContainer");
        this.clearContainer(container);

        const classScope = this;

        if (jobs != null && jobs.length > 0) {
            jobs.forEach(function(job) {
                let status = job.status;
                let date = classScope.formatDate(job.nextRunDue);

                let displayStatus = status === "Paused"
                    ? "Paused"
                    : status === "Enabled"
                    ? `Next due: ${date}`
                    : "Disabled";

                let jobDom =
                    `<tr class="govuk-table__row">
                    <td class="govuk-table__cell">${job.displayName}</td>
                    <td class="govuk-table__cell">
                        ${displayStatus}
                    </td>
                </tr>`;

                container.insertAdjacentHTML("beforeend", jobDom);
            });
        } else {
            let emptyDom =
                `<tr class="govuk-table__row">
			    <td class="govuk-table__cell" colspan="2">No scheduled jobs</td>
		    </tr>`;
            container.insertAdjacentHTML("beforeend", emptyDom);
        }
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
						        <div class="submit-and-clear">
							        <button type="submit" name="jobId" Id="retryJob_${job.jobId}"
                                        onclick="window.prepClient.resubmitJob.call(window.prepClient, ${job.jobId}); return false;">Submit Again</button>
						        </div>
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

    renderMcas(mcadetails) {
        const mcaGlaa = [];

        if (mcadetails && mcadetails.length > 0) {
            for (let i = 0; i < mcadetails.length; i++) {
                const obj = mcadetails[i];
                const value = obj["code"];
                mcaGlaa.push(value);
            }

            this._mcaLabel.textContent = mcaGlaa.join(", ");
        }
    }

    renderSLDDASMismatch(misMatchCount) {
        this._sldDasLabel.textContent = misMatchCount;
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

    setPauseRefJobsButtonState(enabled) {
        setControlEnabledState(enabled, "pause-all-jobs");
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

    setUnPauseReferenceJobsButtonState(enabled) {
        setControlEnabledState(enabled, "resumeReferenceData");
    }

    setPublishMcaReportsButtonState(enabled) {
        setControlEnabledState(enabled, "publishMcaReports");
    }

    setPublishProviderReportsButtonState(enabled) {
        setControlEnabledState(enabled, "publishProviderReports");
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