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
