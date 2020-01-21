﻿import { disabledProceedButtons, jobStatus, jobContinuation } from '/assets/js/periodEnd/state.js';
import { updateSync } from '/assets/js/periodEnd/baseController.js';

class pathController {

    constructor() {
        this._slowTimer = null;
    }

    pathItemCompare(a, b) {
        if (a.ordinal < b.ordinal) {
            return -1;
        }

        if (a.ordinal > b.ordinal) {
            return 1;
        }

        return 0;
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
        }
    }

    isJobContinuable(status) {
        if (status === jobStatus.completed) {
            return jobContinuation.allCompleted;
        }

        if (status === jobStatus.failedRetry || status === jobStatus.failed) {
            return jobContinuation.someFailed;
        }

        return jobContinuation.running;
    }

    getProceedInformationText(enabled, previousItemIsSubPath, nextItemIsSubPath) {
        if (enabled === jobContinuation.running) {
            return "Can't proceed until jobs complete";
        }

        if (nextItemIsSubPath) {
            return "Proceed will start sub-path and run next item";
        }

        if (previousItemIsSubPath) {
            return "Can't proceed as jobs haven't started yet";
        }

        return "";
    }

    renderProceed(pathId, pathItemId, htmlItem, enabled, previousItemIsSubPath, nextItemIsSubPath, collectionYear, period) {
        const proceedEnabled = enabled < jobContinuation.running && !disabledProceedButtons.includes(pathItemId) && !previousItemIsSubPath;

        const node =
            `<li class="app-task-list__item">
                <span class="inline" >
                    <button type="submit" class="proceed" ${proceedEnabled ? "" : "disabled"} Id="proceed_${pathItemId}"
                        onClick="window.periodEndClient.proceed.call(window.periodEndClient, ${collectionYear}, ${period}, ${pathId}, ${pathItemId}); return false;">
                        ${enabled === jobContinuation.someFailed ? "⚠ Proceed with failed jobs" : "Proceed"} ${nextItemIsSubPath ? "↡" : "▼"}
                    </button>
                    <label style="font-size: 10px;">${this.getProceedInformationText(enabled, previousItemIsSubPath, nextItemIsSubPath)}</label>
                </span>
            </li>`;
        htmlItem.insertAdjacentHTML('beforeend', node);
    }

    renderJob(job, jobList) {
        let jobItem = document.createElement("li");
        jobItem.textContent = `Job Id : ${job.jobId}, Status : ${this.jobStatusConvertor(job.status)}`;
        jobList.appendChild(jobItem);

        return this.isJobContinuable(job.status);
    }

    renderSummaryItem(container, path, pathItem) {
        let currentItem = pathItem.ordinal === path.position - 1;

        let item = document.createElement("li");
        item.className += "compact-list-item";

        if (currentItem) {
            let arrow = document.createElement("div");
            arrow.className += "triangle-right";
            container.appendChild(arrow);
        }

        let anchor = document.createElement("a");
        anchor.className += currentItem ? "govuk-link small-link active" : "govuk-link small-link";
        anchor.textContent = pathItem.name;
        anchor.href = "#" + pathItem.name;

        item.appendChild(anchor);

        container.appendChild(item);
    }

    renderPathItem(path, pathItem, subItemList, collectionYear, period) {
        let classScope = this;

        let currentItem = pathItem.ordinal === path.position - 1;

        let totalPathItems = path.pathItems.length;

        let enableProceed = jobContinuation.allCompleted;

        let jobItems = pathItem.pathItemJobs;

        let itemText = pathItem.name;

        if ((pathItem.ordinal < path.position - 1 ||
                (pathItem.ordinal + 1 === path.position && pathItem.ordinal + 1 === totalPathItems)) &&
            (jobItems == undefined || jobItems.length === 0)) {
            itemText += " - Status : Completed";
        }

        let item = document.createElement("li");
        item.className += "app-task-list__item";

        if (currentItem) {
            item.innerHTML = "<b>" + itemText + "</b>";
        } else {
            item.textContent = itemText;
        }

        let itemLink = document.createElement("a");
        itemLink.id = pathItem.name;
        item.appendChild(itemLink);

        if (jobItems != undefined && jobItems.length > 0) {
            let jobList = document.createElement("ul");
            for (let job of jobItems) {
                let status = classScope.renderJob(job, jobList);
                if (status === jobContinuation.running) {
                    enableProceed = jobContinuation.running;
                }

                if (enableProceed !== jobContinuation.running && status === jobContinuation.someFailed) {
                    enableProceed = jobContinuation.someFailed;
                }
            }

            let jobItemSummary = pathItem.pathItemJobSummary;
            if (jobItemSummary != undefined) {
                let jobSummary = document.createElement("li");
                jobSummary.className += "app-task-list__summary";
                jobSummary.textContent = `${jobItemSummary.numberOfWaitingJobs} Waiting, ${jobItemSummary.numberOfRunningJobs} Running, ${jobItemSummary.numberOfFailedJobs} Failed, ${jobItemSummary.numberOfCompleteJobs} Complete`;
                jobList.appendChild(jobSummary);
            }

            item.appendChild(jobList);
        }

        let summary = pathItem.pathItemJobSummary;
        if (summary != undefined && summary != null) {
            if (summary.numberOfWaitingJobs + summary.numberOfRunningJobs > 0) {
                enableProceed = jobContinuation.running;
            }
            else if (summary.numberOfFailedJobs === 0) {
                enableProceed = jobContinuation.allCompleted;
            } else {
                enableProceed = jobContinuation.someFailed;
            }
        }

        subItemList.appendChild(item);

        if (currentItem) {
            if (pathItem.ordinal + 1 !== totalPathItems) {
                this.renderProceed(path.pathId, pathItem.pathItemId, subItemList, enableProceed, pathItem.subPaths !== null, path.pathItems[pathItem.ordinal + 1].subPaths !== null, collectionYear, period);
            }
        }
    }

    setButtonState(enabled, buttonId) {
        const button = document.getElementById(buttonId);
        if (button != null) {
            button.disabled = !enabled;
        }
    }

    disableProceed(pathItemId) {
        const button = document.getElementById("proceed_" + pathItemId);
        if (button != null) {
            disabledProceedButtons.push(pathItemId);
            button.disabled = true;
        }
    }
    
    padLeft(str, padString, max) {
        str = str.toString();
        return str.length < max ? this.padLeft(padString + str, padString, max) : str;
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    renderPaths(state) {
        updateSync.call(this);

        if (state === "" || state === undefined) {
            return;
        }

        const stateModel = typeof state === 'object' ? state : JSON.parse(state);

        let pathContainer = document.getElementById("pathContainer");
        while (pathContainer.firstChild) {
            pathContainer.removeChild(pathContainer.firstChild);
        }

        let summaryContainer = document.getElementById("summaryContainer");
        while (summaryContainer.firstChild) {
            summaryContainer.removeChild(summaryContainer.firstChild);
        }

        let classScope = this;
        stateModel.paths.forEach(function (path) {
            let period = path.period;
            let collectionYear = path.collectionYear;

            let pathItems = path.pathItems;

            let pathSummaryTitle = document.createElement("span");
            pathSummaryTitle.className += "govuk-heading-s";
            pathSummaryTitle.textContent = path.name;

            let pathSummary = document.createElement("span");
            pathSummary.className += "nav";
            let pathSummaryList = document.createElement("ul");
            pathSummaryList.className += "govuk-list";

            if (pathItems != undefined && pathItems.length > 0) {
                pathItems.sort(classScope.pathItemCompare);

                let li = document.createElement("li");

                let title = document.createElement("h2");
                title.textContent = path.name;
                li.appendChild(title);

                let subItemList = document.createElement("ul");
                subItemList.className += "app-task-list__items";

                pathItems.forEach(function (pathItem) {
                    if (pathItem.name != undefined && pathItem.name !== "") {
                        classScope.renderPathItem(path, pathItem, subItemList, collectionYear, period);
                        classScope.renderSummaryItem(pathSummaryList, path, pathItem);
                    }
                });

                pathSummary.appendChild(pathSummaryList);

                summaryContainer.appendChild(pathSummaryTitle);
                summaryContainer.appendChild(pathSummary);

                li.appendChild(subItemList);

                pathContainer.appendChild(li);
            }
        });
    }

    initialiseState(state) {
        const mcaEnabled = state.collectionClosed && state.mcaReportsReady && !state.mcaReportsPublished;
        const providerEnabled = state.collectionClosed && state.providerReportsReady && !state.providerReportsPublished;
        const reportsFinished = !mcaEnabled && !providerEnabled && state.mcaReportsReady && state.providerReportsReady;

        this.setButtonState(state.collectionClosed && !state.periodEndStarted, "startPeriodEnd");
        this.setButtonState(mcaEnabled, "publishMcaReports");
        this.setButtonState(providerEnabled, "publishProviderReports");
        this.setButtonState(state.collectionClosed && reportsFinished && !state.periodEndFinished, "closePeriodEnd");
        this.setButtonState(state.collectionClosed && state.periodEndFinished && state.referenceDataJobsPaused, "resumeReferenceData");
    }
}

export default pathController;