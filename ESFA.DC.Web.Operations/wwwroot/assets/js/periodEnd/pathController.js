import { disabledProceedButtons } from '/assets/js/periodEnd/state.js';

class pathController {

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
            case 1:
                return "Ready";
            case 2:
                return "Moved For Processing";
            case 3:
                return "Processing";
            case 4:
                return "Completed";
            case 5:
                return "Failed Retry";
            case 6:
                return "Failed";
            case 7:
                return "Paused";
            case 8:
                return "Waiting";
            default:
        }
    }

    isJobContinuable(jobStatus) {
        if (jobStatus === 4) {
            return 1;
        }

        if (jobStatus === 5 || jobStatus === 6) {
            return 2;
        }

        return 0;
    }

    renderProceed(pathId, pathItemId, htmlItem, enabled, hasSubPaths, collectionYear, period) {
        const proceedEnabled = enabled > 0 && !disabledProceedButtons.includes(pathItemId);

        const node =
            `<li class="app-task-list__item">
                <span class="inline" >
                    <button type="submit" class="proceed" ${proceedEnabled ? "" : "disabled"} Id="proceed_${pathItemId}"
                        onClick="window.periodEndClient.proceed.call(window.periodEndClient, ${collectionYear}, ${period}, ${pathId}, ${pathItemId}); return false;">
                        ${enabled === 2 ? "⚠ Proceed with failed jobs" : "Proceed"} ${hasSubPaths ? "↡" : "▼"}
                    </button>
                    <label style="font-size: 10px;">${hasSubPaths ? "Proceed will start sub-path and run next item" : ""}</label>
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

        let enableProceed = 1;

        let jobItems = pathItem.pathItemJobs;

        let itemText = pathItem.name;

        if ((pathItem.ordinal < path.position - 1 ||
                (pathItem.ordinal + 1 === path.position && pathItem.ordinal + 1 === totalPathItems)) &&
            (jobItems == undefined || jobItems.length === 0)) {
            itemText += " - Status : Completed";
        }

        let item = document.createElement("li");
        item.className += "app-task-list__item";
        // item.innerHTML = "<b>" + pathItem.name + "</b>";

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
            for (const job of jobItems) {
                let status = classScope.renderJob(job, jobList);
                if (status === 0) {
                    enableProceed = 0;
                    break;
                }

                if (status === 2) {
                    enableProceed = 2;
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
        if (summary != undefined) {
            if (summary.numberOfWaitingJobs + summary.numberOfRunningJobs > 0) {
                enableProceed = 0;
            }
            else if (summary.numberOfFailedJobs === 0) {
                enableProceed = 1;
            } else {
                enableProceed = 2;
            }
        }

        subItemList.appendChild(item);

        if (currentItem) {
            if (pathItem.ordinal + 1 !== totalPathItems) {
                // debugger;
                this.renderProceed(path.pathId, pathItem.pathItemId, subItemList, enableProceed, path.pathItems[pathItem.ordinal + 1].subPaths !== null, collectionYear, period);
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

    updateSync() {
        let date = new Date();
        let day = this.padLeft(date.getDate(), "0", 2);
        let month = this.padLeft(date.getMonth() + 1, "0", 2);

        let hours = this.padLeft(date.getHours(), "0", 2);
        let minutes = this.padLeft(date.getMinutes(), "0", 2);
        let seconds = this.padLeft(date.getSeconds(), "0", 2);

        const dateLabel = document.getElementById("lastSync");
        dateLabel.textContent = `Last updated: ${day}/${month}/${date.getFullYear()} ${hours}:${minutes}:${seconds}`;
    }

    padLeft(str, padString, max) {
        str = str.toString();
        return str.length < max ? this.padLeft(padString + str, padString, max) : str;
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    renderPaths(pathString) {
        this.updateSync();

        if (pathString === "" || pathString === undefined) {
            return;
        }

        const paths = JSON.parse(pathString);

        let pathContainer = document.getElementById("pathContainer");
        while (pathContainer.firstChild) {
            pathContainer.removeChild(pathContainer.firstChild);
        }

        let summaryContainer = document.getElementById("summaryContainer");
        while (summaryContainer.firstChild) {
            summaryContainer.removeChild(summaryContainer.firstChild);
        }

        let classScope = this;
        paths.forEach(function (path) {
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
        const periodClosed = state.collectionClosed === "True";
        const mcaEnabled = periodClosed && state.mcaReportsReady === "True" && !(state.mcaReportsPublished === "True");
        const providerEnabled = periodClosed && state.providerReportsReady === "True" && !(state.providerReportsPublished === "True")
        const reportsFinished = !mcaEnabled &&
            !providerEnabled &&
            state.mcaReportsReady === "True" &&
            state.providerReportsReady === "True";

        this.setButtonState(periodClosed && !(state.periodEndStarted === "True"), "startPeriodEnd");
        this.setButtonState(mcaEnabled, "publishMcaReports");
        this.setButtonState(providerEnabled, "publishProviderReports");
        this.setButtonState(periodClosed && reportsFinished && !(state.periodEndFinished === "True"), "closePeriodEnd");
        this.setButtonState(periodClosed && state.periodEndFinished === "True" && state.referenceDataJobsPaused === "True", "resumeReferenceData");
    }
}

export default pathController;