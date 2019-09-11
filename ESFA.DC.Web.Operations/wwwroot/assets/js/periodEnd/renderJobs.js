﻿"use strict";

function renderJobs(referenceJobs, failedJobs) {
    renderFailedJobs(failedJobs);
    renderReferenceJobs(referenceJobs);
}

function renderReferenceJobs(jobsString) {

    if (jobsString === "" || jobsString === undefined) {
        return;
    }

    let jobs = JSON.parse(jobsString);
    
    let container = document.getElementById("referenceDataContainer");
    clearContainer(container);

    if (jobs != null && jobs.length > 0) {
        jobs.forEach(function(job) {
            let status = job.status;
            let date = formatDate(job.nextRunDue);

            let displayStatus = status === "Paused"
                ? "Paused"
                : status === "Enabled" ? `Next due: ${date}` : "Disabled";
            
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

function renderFailedJobs(jobsString) {

    if (jobsString === "" || jobsString === undefined) {
        return;
    }

    let jobs = JSON.parse(jobsString);

    let container = document.getElementById("failedJobContainer");
    clearContainer(container);

    if (jobs != null && jobs.length > 0) {
        jobs.forEach(function(job) {
            let provider = job.ukprn + " - " + job.organisationName;

            let date = formatDate(job.dateTimeSubmitted);
            let jobDom =
                `<div class="main-card">
				<div class="flex govuk-!-margin-bottom-2">
					<strong class="govuk-link govuk-!-font-size-19 provider-title ">${provider}</strong>
				</div>
				<div class="govuk-body card-wrapper ilr">
					<div class="flex space-between govuk-!-margin-bottom-2">
						<span class="wrong-file-name">${job.collectionName}</span>
						<div class="submit-and-clear">
							<button type="submit" name="jobId" value="${job.jobId}">Submit Again</button>
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

function clearContainer(container) {
    while (container.firstChild) {
        container.removeChild(container.firstChild);
    }
}

function formatDate(dateString) {
    let dateOptions = { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: 'numeric' };

    return  new Date(dateString).toLocaleDateString("en-GB", dateOptions);
}