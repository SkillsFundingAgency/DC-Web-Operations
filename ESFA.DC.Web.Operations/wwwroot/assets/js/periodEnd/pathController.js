import { jobStatus, jobContinuation } from '/assets/js/periodEnd/state.js';
import { updateSync } from '/assets/js/periodEnd/baseController.js';
import { removeSpaces } from '/assets/js/util.js';

class pathController {

    constructor() {
        this._slowTimer = null;
        this._year = 0;
        this._period = 0;
        this.lastMessage = null;
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
            return " Proceed will start sub-path(s) and run next item";
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

    renderSummaryItem(container, path, pathItem) {
        if ((path.pathId === 0 && (pathItem.ordinal < path.position - 3 || pathItem.ordinal > path.position + 1)) || (path.pathId > 0 && path.position !== -1 && (pathItem.ordinal < path.position - 2 || pathItem.ordinal > path.position))) {
            return;
        }

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

    renderJobs(item, jobItems, pathItemName, pathItemSummary) {
        let jobStatus = jobContinuation.nothingRunning;

        if (jobItems != undefined && jobItems.length > 0) {
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

        if ((pathItem.ordinal < path.position - 1 ||
                (pathItem.ordinal + 1 === path.position && pathItem.ordinal + 1 === totalPathItems)) &&
            (jobItems === undefined || jobItems === null || jobItems.length === 0)) {
            itemText += " - Status : Completed";
        }

        let item = document.createElement("li");
        item.id = removeSpaces(pathItem.name);
        item.className += "app-task-list__item";

        if (pathItem.subPaths !== null) {
            itemText = `${itemText} <a href="#Path${pathItem.subPaths[0]}">❯</a>`;
        }

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

    renderPaths(state) {
        updateSync.call(this);

        if (state === "" || state === undefined) {
            return;
        }

        const stateModel = typeof state === 'object' ? state : JSON.parse(state);

        if (this.lastMessage !== null) {
            this.updatePaths(stateModel);
            return;
        }

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
            let pathItems = path.pathItems;

            let pathSummaryTitle = document.createElement("a");
            pathSummaryTitle.className += "govuk-heading-s";
            pathSummaryTitle.textContent = path.name;
            pathSummaryTitle.href = "#Path" + path.pathId;

            let pathSummary = document.createElement("span");
            pathSummary.className += "nav";
            let pathSummaryList = document.createElement("ul");
            pathSummaryList.id = `ST-${removeSpaces(path.name)}`;
            pathSummaryList.className += "govuk-list";

            if (pathItems != undefined && pathItems.length > 0) {
                pathItems.sort(classScope.pathItemCompare);

                let li = document.createElement("li");

                let itemLink = document.createElement("a");
                itemLink.id = `Path${path.pathId}`;
                li.appendChild(itemLink);

                let title = document.createElement("h3");
                title.textContent = path.name;

                let pathFound = true;
                if (path.pathId !== 0) {
                    let anchorTarget = classScope.getPathItemName(stateModel, path.pathId);
                    if (anchorTarget === "Path0") {
                        pathFound = false;
                    }

                    let anchor = document.createElement("a");
                    anchor.textContent = "❮ ";
                    anchor.href = "#" + anchorTarget;
                    title.prepend(anchor);
                }

                if (pathFound === true) {
                    li.appendChild(title);
                } else {
                    let strike = document.createElement("strike");
                    strike.appendChild(title);
                    li.appendChild(strike);
                }

                let subItemList = document.createElement("ul");
                subItemList.className += "app-task-list__items";
                subItemList.id = `PI-${removeSpaces(path.name)}`;

                pathItems.forEach(function (pathItem) {
                    if (pathItem.name != undefined && pathItem.name !== "") {
                        classScope.renderPathItem.call(classScope, path, pathItem, subItemList);
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

        this.lastMessage = stateModel;
    }

    updatePaths(stateModel) {
        let classScope = this;
        stateModel.paths.forEach(function(path) {
            const oldPath = classScope.getPath(classScope.lastMessage, path.pathId);

            if (path.position !== oldPath.position) {
                classScope.renderSummaryPath(path);
                
                let i = Math.max(oldPath.position - 1, 0), len = path.position, original = i, proceed = path.position - 1;
                let pathList = document.getElementById(`PI-${removeSpaces(path.name)}`);
                for (; i < len; i++) {
                    if (original === i || proceed === i || !classScope.pathItemsSame(path.pathItems[i], oldPath.pathItems[i], true)) {
                        console.log(`Rendering ${path.pathItems[i].name} as position has incremented`);
                        classScope.deleteItem(pathList, path.pathItems[i].name);
                        classScope.renderPathItem(path, path.pathItems[i], pathList);
                    }
                }
            } else {
                if (path.position > 0 && !classScope.pathItemsSame(path.pathItems[path.position - 1], oldPath.pathItems[oldPath.position - 1], false)) {
                    console.log(`Rendering ${path.pathItems[path.position - 1].name} as item has changed`);
                    let pathList = document.getElementById(`PI-${removeSpaces(path.name)}`);
                    classScope.deleteItem(pathList, path.pathItems[path.position - 1].name);
                    classScope.renderPathItem(path, path.pathItems[path.position - 1], pathList);
                }
            }
        });

        this.lastMessage = stateModel;
    }

    deleteItem(pathList, pathItemName) {
        let itemToDelete = document.getElementById(removeSpaces(pathItemName));
        while (itemToDelete.firstChild) {
            itemToDelete.removeChild(itemToDelete.firstChild);
        }

        pathList.removeChild(itemToDelete);
    }

    renderSummaryPath(path) {
        let classScope = this;
        let summaryList = document.getElementById(`ST-${removeSpaces(path.name)}`);
        while (summaryList.firstChild) {
            summaryList.removeChild(summaryList.firstChild);
        }

        let pathItems = path.pathItems;
        if (pathItems != undefined && pathItems.length > 0) {
            pathItems.sort(classScope.pathItemCompare);
            pathItems.forEach(function (pathItem) {
                if (pathItem.name != undefined && pathItem.name !== "") {
                    classScope.renderSummaryItem(summaryList, path, pathItem);
                }
            });
        }
    }

    pathItemsSame(pathItemOne, pathItemTwo, positionChanged) {
        if ((pathItemOne.pathItemJobs !== null && pathItemTwo.pathItemJobs === null) ||
            (pathItemOne.pathItemJobs === null && pathItemTwo.pathItemJobs !== null)) {
            return false;
        }

        if ((pathItemOne.pathItemJobSummary === null && pathItemTwo.pathItemJobSummary !== null) ||
            (pathItemOne.pathItemJobSummary !== null && pathItemTwo.pathItemJobSummary === null)) {
            return false;
        }

        if (pathItemOne.pathItemJobs !== null && pathItemTwo.pathItemJobs !== null) {
            if (pathItemOne.pathItemJobs.length !== pathItemTwo.pathItemJobs.length) {
                return false;
            }

            let i = 0, len = pathItemOne.pathItemJobs.length;
            for (; i < len; i++) {
                if (pathItemOne.pathItemJobs[i].jobId !== pathItemTwo.pathItemJobs[i].jobId ||
                    pathItemOne.pathItemJobs[i].status !== pathItemTwo.pathItemJobs[i].status) {
                    return false;
                }
            }
        }

        if (pathItemOne.pathItemJobSummary !== null && pathItemTwo.pathItemJobSummary !== null) {
            return pathItemOne.pathItemJobSummary.numberOfWaitingJobs === pathItemTwo.pathItemJobSummary.numberOfWaitingJobs
                && pathItemOne.pathItemJobSummarynumberOfRunningJobs === pathItemTwo.pathItemJobSummarynumberOfRunningJobs
                && pathItemOne.pathItemJobSummary.numberOfFailedJobs === pathItemTwo.pathItemJobSummary.numberOfFailedJobs
                && pathItemOne.pathItemJobSummary.numberOfCompleteJobs === pathItemTwo.pathItemJobSummary.numberOfCompleteJobs;
        }

        if (positionChanged && pathItemOne.pathItemJobs === null) {
            return false;
        }

        return true;
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
        const mcaEnabled = state.collectionClosed && state.mcaReportsReady && !state.mcaReportsPublished;
        const providerEnabled = state.collectionClosed && state.providerReportsReady && !state.providerReportsPublished;
        const reportsFinished = !mcaEnabled && !providerEnabled && state.mcaReportsReady && state.providerReportsReady;

        this.setButtonState(state.collectionClosed && !state.periodEndStarted, "startPeriodEnd");
        this.setButtonState(mcaEnabled, "publishMcaReports");
        this.setButtonState(providerEnabled, "publishProviderReports");
        this.setButtonState(state.collectionClosed && reportsFinished && !state.periodEndFinished, "closePeriodEnd");
        this.setButtonState(state.collectionClosed && state.periodEndFinished && state.referenceDataJobsPaused, "resumeReferenceData");

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