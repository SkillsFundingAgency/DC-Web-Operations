﻿import { jobStatus, jobContinuation } from '/assets/js/periodEnd/state.js';
import { updateSync } from '/assets/js/periodEnd/baseController.js';
import { removeSpaces } from '/assets/js/util.js';

class pathController {

    constructor() {
        this._slowTimer = null;
        this._year = 0;
        this._period = 0;
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
                return "";
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

    getProceedInformationText(jobState, itemIsSubPath, nextItemIsSubPath) {
        if (jobState === jobContinuation.running) {
            return " Can't proceed until jobs complete";
        }

        if (nextItemIsSubPath) {
            return " Proceed will start sub-path(s) and run next item(s)";
        }

        if (itemIsSubPath && jobState === jobContinuation.nothingRunning) {
            return " Can't proceed as jobs haven't started yet";
        }

        return "";
    }

    renderProceed(pathId, pathItemId, jobState, itemIsSubPath, nextItemIsSubPath) {
        const proceedEnabled = (!itemIsSubPath && jobState < jobContinuation.running) || (itemIsSubPath === true && jobState !== jobContinuation.running);

        let proceedLi = document.createElement("li");
        proceedLi.className = "app-task-list__item";
        proceedLi.id = `PL_${pathItemId}`;

        let span = document.createElement("span");
        span.className = "inline";

        let button = document.createElement("button");
        button.type = "submit";
        button.className = "proceed";
        button.id = `proceed_${pathItemId}`;
        if (window.periodEndClient !== undefined) {
            button.addEventListener("click",
                window.periodEndClient.proceed.bind(window.periodEndClient,
                    this._year,
                    this._period,
                    pathId,
                    pathItemId));
        }

        button.textContent = `${jobState === jobContinuation.someFailed ? "⚠ Proceed with failed jobs" : "Proceed"} ${nextItemIsSubPath ? "↡" : "▼"}`;
        if (!proceedEnabled) {
            button.disabled = true;
        }

        let label = document.createElement("label");
        label.style = "font-size: 10px;";
        label.textContent = `${this.getProceedInformationText(jobState, itemIsSubPath, nextItemIsSubPath)}`;

        span.appendChild(button);
        span.appendChild(label);
        proceedLi.appendChild(span);

        return proceedLi;
    }

    renderJob(job, jobList) {
        let jobItem = document.createElement("li");
        jobItem.textContent = this.getJobText(job);

        let retryLink = document.createElement("a");
        retryLink.text = "Retry";
        retryLink.href = "#/";
        retryLink.className = "govuk-link govuk-!-margin-left-3";
        retryLink.id = `retryJob_${job.jobId}`;
        if (window.periodEndClient !== undefined) {
            retryLink.addEventListener("click",
                window.periodEndClient.resubmitJob.bind(window.periodEndClient,
                    job.jobId));
        }

        if (job.status === jobStatus.failed || job.status === jobStatus.failedRetry) {
            jobItem.append(retryLink);
        }

        jobList.appendChild(jobItem);

        return this.isJobContinuable(job.status);
    }

    getJobText(job) {
        let text = `Job Id : ${job.jobId}, `;

        if (job.providerName !== "0") {
            text += `Provider : ${job.providerName}, `;
        }

        text += `Status : ${this.jobStatusConvertor(job.status)}`;
        return text;
    }

    renderJobs(item, jobItems, pathItemName, pathItemSummary) {
        let jobStatus = jobContinuation.nothingRunning;

        if (jobItems !== undefined && jobItems !== null && jobItems.length > 0) {
            let jobList = document.createElement("ul");
            jobList.id = `JL-${pathItemName}`;
            for (let job of jobItems) {
                let status = this.renderJob(job, jobList);
                if (status === jobContinuation.running) {
                    jobStatus = jobContinuation.running;
                }

                if (jobStatus !== jobContinuation.running && status === jobContinuation.someFailed) {
                    jobStatus = jobContinuation.someFailed;
                }

                if (jobStatus !== jobContinuation.running && jobStatus !== jobContinuation.someFailed && status === jobContinuation.allCompleted) {
                    jobStatus = jobContinuation.allCompleted;
                }
            }

            if (pathItemSummary !== undefined && pathItemSummary != null) {
                let jobSummary = document.createElement("li");
                jobSummary.id = `JS-${pathItemName}`;
                jobSummary.className += "app-task-list__summary";
                jobSummary.textContent = `${pathItemSummary.numberOfWaitingJobs} Waiting, ${pathItemSummary.numberOfRunningJobs} Running, ${pathItemSummary.numberOfFailedJobs} Failed, ${pathItemSummary.numberOfCompleteJobs} Complete`;
                jobList.appendChild(jobSummary);

                if (pathItemSummary.numberOfWaitingJobs + pathItemSummary.numberOfRunningJobs > 0) {
                    jobStatus = jobContinuation.running;
                }
                else if (pathItemSummary.numberOfFailedJobs === 0) {
                    jobStatus = jobContinuation.allCompleted;
                } else {
                    jobStatus = jobContinuation.someFailed;
                }
            }

            item.appendChild(jobList);
        }

        return jobStatus;
    }

    renderPathItem(path, pathItem, subItemList) {
        let classScope = this;

        let currentItem = pathItem.ordinal === path.position - 1;

        let totalPathItems = path.pathItems.length;
        
        let jobItems = pathItem.pathItemJobs;

        let itemText = pathItem.name;

        if (!pathItem.hasJobs && pathItem.ordinal <= path.position - 1) {
            itemText += " - Status : Completed";
        }

        let item = document.createElement("li");
        item.id = removeSpaces(pathItem.name);
        item.className += "app-task-list__item";

        if (currentItem) {
            itemText = `<b>${itemText}</b>`;
        }

        item.innerHTML = itemText;

        let itemLink = document.createElement("a");
        itemLink.id = pathItem.name;
        item.appendChild(itemLink);

        const jobStatus = this.renderJobs(item, jobItems, removeSpaces(pathItem.name), pathItem.pathItemJobSummary);

        if (currentItem && pathItem.ordinal + 1 !== totalPathItems) {
            let panel = document.createElement("div");

            item.id = `PA-${item.id}`;
            panel.id = removeSpaces(pathItem.name);
            panel.className += "app-task-list__item";
            panel.appendChild(item);

            panel.appendChild(this.renderProceed.call(classScope,
                path.pathId,
                pathItem.pathItemId,
                jobStatus,
                pathItem.subPaths !== null,
                path.pathItems[pathItem.ordinal + 1].subPaths !== null));
            subItemList.insertBefore(panel, subItemList.children[pathItem.ordinal]);
        } else {
            subItemList.insertBefore(item, subItemList.children[pathItem.ordinal]);
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
            button.disabled = true;
        }
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
        let currentContent = fileContainer.innerHTML;
        let classScope = this;
        let updatedContent = currentContent;
        stateModel.files.forEach(function(file) {
            const content = 
                `<tr class="govuk-table__row">
                    <td class="govuk-table__cell"><a href="/periodendallf/periodEnd/getReportFile/${file.fileName}">${file.fileName}</a></td>
                    <td class="govuk-table__cell">
                        <span>${classScope.jobStatusConvertor(file.jobStatus)}</span> <br />
                        <span class="govuk-!-font-weight-bold">${file.recordCount} records</span> <br />
                        <span class="govuk-!-font-weight-bold">${file.errorCount} errors</span> <br />
                    </td>
                    <td class="govuk-table__cell"><a href="/periodendallf/periodEnd/getReportFile/${file.reportName}/${stateModel.period}/${file.jobId}">${file.reportName}</a></td>
                </tr>`;
            updatedContent += content;
        });

        fileContainer.innerHTML = updatedContent;
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

        let classScope = this;
        stateModel.paths.forEach(function (path) {
            let pathItems = path.pathItems;

            if (pathItems != undefined && pathItems.length > 0) {
                pathItems.sort(classScope.pathItemCompare);

                let li = document.createElement("li");

                let itemLink = document.createElement("a");
                itemLink.id = `Path${path.pathId}`;
                li.appendChild(itemLink);

                let title = document.createElement("h3");
                title.textContent = path.name;

                li.appendChild(title);

                let subItemList = document.createElement("ul");
                subItemList.className += "app-task-list__items";
                subItemList.id = `PI-${removeSpaces(path.name)}`;

                pathItems.forEach(function (pathItem) {
                    if (pathItem.name) {
                        classScope.renderPathItem.call(classScope, path, pathItem, subItemList);
                    }
                });

                li.appendChild(subItemList);

                pathContainer.appendChild(li);
            }
        });
    }

    deleteItem(pathList, pathItemName) {
        let itemToDelete = document.getElementById(removeSpaces(pathItemName));
        while (itemToDelete.firstChild) {
            itemToDelete.removeChild(itemToDelete.firstChild);
        }

        pathList.removeChild(itemToDelete);
    }

    getPath(stateModel, pathId) {
        let i = 0, len = stateModel.paths.length;
        for (; i < len; i++) {
            if (stateModel.paths[i].pathId === pathId) {
                return stateModel.paths[i];
            }
        }

        return null;
    }

    initialiseState(state) {

        this.setButtonState(state.collectionClosed && !state.periodEndStarted, "startPeriodEnd");
        this.setButtonState(state.collectionClosed && !state.periodEndFinished && state.closePeriodEndEnabled, "closePeriodEnd");

        this._year = state.year;
        this._period = state.period;
    }

    getPathItemName(stateModel, pathId) {
        let i = 0, len = stateModel.paths[0].pathItems.length;
        for (; i < len; i++) {
            if (stateModel.paths[0].pathItems[i].subPaths !== null &&
                stateModel.paths[0].pathItems[i].subPaths[0] === pathId) {
                return stateModel.paths[0].pathItems[i].name;
            }
        }

        return "Path0";
    }
}

export default pathController;