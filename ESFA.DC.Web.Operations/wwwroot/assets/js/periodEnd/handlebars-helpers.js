import { jobStatusConvertor, getJobContinuationStatus } from './jobUtils.js';
import { jobContinuation, jobStatus } from '/assets/js/periodEnd/state.js';

export let Templates = {
    ILRPeriodEnd: 'PeriodEnd/ILRPeriodEnd.html',
    ProceedButton: 'PeriodEnd/ProceedButton.html',
    PathItemJobSummary: 'PeriodEnd/PathItemJobSummary.html',
    ILRPeriodEndNavigation: 'PeriodEnd/ILRPeriodEndNavigation.html'
};

Handlebars.registerHelper('getProviderName', function (providerName) {
    return providerName !== "0" ? `Provider : ${providerName}, ` : '';
});

Handlebars.registerHelper('getStatus', function (status) {
    return jobStatusConvertor(status);
});

Handlebars.registerHelper('is_current', function (pathItemOrdinal, pathPosition) {
    return pathItemOrdinal === pathPosition - 1;
});

Handlebars.registerHelper('is_past', function (pathItemOrdinal, pathPosition) {
    return pathItemOrdinal < pathPosition - 1;
});

Handlebars.registerHelper('can_retry', function (status) {
    return status === jobStatus.failed || status === jobStatus.failedRetry;
});

Handlebars.registerHelper('getProceedButtonText', function (pathItemJobs) {
    if (pathItemJobs && pathItemJobs.some(j => j.status === jobStatus.failed || j.status === jobStatus.failedRetry)){
        return "⚠ Proceed with failed jobs ▼";
    }

    return "Proceed ▼";
});

Handlebars.registerHelper('get_path_name_by_sub_path_id', function (stateModel, pathId) {
    let i = 0, len = stateModel.paths[0].pathItems.length;
    for (; i < len; i++) {
        if (stateModel.paths[0].pathItems[i].subPaths !== null &&
            stateModel.paths[0].pathItems[i].subPaths[0] === pathId) {
            return stateModel.paths[0].pathItems[i].name;
        }
    }
    return "Path0";
});


Handlebars.registerHelper('can_continue', function (pathItemJobs) {
    const continueStatus = getJobContinuationStatus(pathItemJobs);
    return continueStatus === jobContinuation.allCompleted || continueStatus === jobContinuation.someFailed;
});

Handlebars.registerHelper('is_subPath', function (path) {
    return path.subPaths ? true : false;
});

Handlebars.registerHelper('getProceedLabelText', function (pathItem) {
    const continueStatus = getJobContinuationStatus(pathItem.pathItemJobs);
    if (continueStatus === jobContinuation.running) {
        return " Can't proceed until jobs complete";
    }

    //TODO
    //if (nextItemIsSubPath) {
    //    return " Proceed will start sub-path(s) and run next item(s)";
    //}

    if (pathItem.subPath === null && continueStatus === jobContinuation.nothingRunning) {
        return " Can't proceed as jobs haven't started yet";
    }

    return "";
});

Handlebars.registerHelper('include_in_nav', function (pathItem, path) {
    if ((path.pathId === 0
        && (pathItem.ordinal < path.position - 3 || pathItem.ordinal > path.position + 1))
        || (path.pathId > 0 && path.position !== -1 && (pathItem.ordinal < path.position - 2 || pathItem.ordinal > path.position))) {
        return false;
    }
    return true;
});

