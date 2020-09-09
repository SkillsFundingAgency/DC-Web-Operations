import { jobStatus, jobContinuation } from '/assets/js/periodEnd/state.js';

export function jobStatusConvertor(status) {
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

export function getJobContinuationStatus(pathItemsJobs) {

    if (pathItemsJobs) {
        if (pathItemsJobs.some(j => j.status === jobStatus.processing
            || j.status === jobStatus.ready
            || j.status === jobStatus.movedForProcessing
            || j.status === jobStatus.paused
            || j.status === jobStatus.waiting)) {
            return jobContinuation.running;
        }

        if (pathItemsJobs.some(j => j.status === jobStatus.failed || j.status === jobStatus.failedRetry)) {
            return jobContinuation.someFailed;
        };

        if (pathItemsJobs.every(j => j.status === jobStatus.completed)) {
            return jobContinuation.allCompleted;
        };
    }

    return jobContinuation.nothingRunning;
}


export function getProviderName(providerName) {
    return providerName !== "0" ? `Provider : ${providerName}, ` : '';
}

export function getStatus(status) {
    return jobStatusConvertor(status);
}

export function isCurrent(pathItemOrdinal, pathPosition) {
    return pathItemOrdinal === pathPosition - 1;
}

export function isPast(pathItemOrdinal, pathPosition) {
    return pathItemOrdinal < pathPosition - 1;
}

export function isCompleted(pathItem, path) {
    const jobItems = pathItem.pathItemJobs;
    if ((pathItem.ordinal < path.position - 1 ||
        (pathItem.ordinal + 1 === path.position && pathItem.ordinal + 1 === path.pathItems.length)) && 
        (jobItems === undefined || jobItems === null || jobItems.length === 0)) {
        return true;
    }
    return false;
}
export function canRetry(status) {
    return status === jobStatus.failed || status === jobStatus.failedRetry;
}

export function getProceedButtonText(pathItemJobs, isNextItemSubPath) {
    if (pathItemJobs && pathItemJobs.some(j => j.status === jobStatus.failed || j.status === jobStatus.failedRetry)) {
        return "⚠ Proceed with failed jobs ▼";
    }

    if (isNextItemSubPath) {
        return "Proceed ↡";
    }
    return "Proceed ▼";
}

export function getPathNameBySubPathId(stateModel, pathId) {
    let i = 0, len = stateModel.paths[0].pathItems.length;
    for (; i < len; i++) {
        if (stateModel.paths[0].pathItems[i].subPaths !== null &&
            stateModel.paths[0].pathItems[i].subPaths[0] === pathId) {
            return stateModel.paths[0].pathItems[i].name;
        }
    }
    return "Path0";
}


export function canContinue(pathItemJobs) {
    const continueStatus = getJobContinuationStatus(pathItemJobs);
    return continueStatus === jobContinuation.allCompleted || continueStatus === jobContinuation.someFailed;
}

export function isSubPath(path) {
    return path.subPaths ? true : false;
}

export function getProceedLabelText(pathItem, nextItemIsSubPath) {
    const continueStatus = getJobContinuationStatus(pathItem.pathItemJobs);
    if (continueStatus === jobContinuation.running) {
        return " Can't proceed until jobs complete";
    }

    if (nextItemIsSubPath) {
        return " Proceed will start sub-path(s) and run next item(s)";
    }

    if (pathItem.subPath === null && continueStatus === jobContinuation.nothingRunning) {
        return " Can't proceed as jobs haven't started yet";
    }

    return "";
}

export function includeInNav(pathItem, path) {
    if ((path.pathId === 0
        && (pathItem.ordinal < path.position - 3 || pathItem.ordinal > path.position + 1))
        || (path.pathId > 0 && path.position !== -1 && (pathItem.ordinal < path.position - 2 || pathItem.ordinal > path.position))) {
        return false;
    }
    return true;
}

export function isNextItemSubPath(pathItems, index) {
    const nextItem = pathItems[index + 1];
    if (nextItem) {
        return nextItem.subPaths ? true : false;
    }
    return false;
}


