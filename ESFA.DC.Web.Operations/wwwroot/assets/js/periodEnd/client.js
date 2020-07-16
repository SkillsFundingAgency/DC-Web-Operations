import JobController from '/assets/js/periodEnd/jobController.js';

class client {
    
    constructor(connection) {
        this.connection = connection;
        this.jobController = new JobController();
    }

    startPeriodEnd(collectionYear, period, collectionType) {
        this.jobController.setStartPeriodEndButtonState(false);
        this.invokeActionWithType("StartPeriodEnd", collectionYear, period, collectionType);
    }

    unPauseReferenceJobs(collectionYear, period) {
        this.jobController.setUnPauseReferenceJobsButtonState(false);
        this.invokeAction("UnPauseReferenceJobs", collectionYear, period);
    }

    publishProviderReports(collectionYear, period) {
        this.jobController.setPublishProviderReportsButtonState(false);
        this.invokeAction("PublishProviderReports", collectionYear, period);
    }

    publishMcaReports(collectionYear, period) {
        this.jobController.setPublishMcaReportsButtonState(false);
        this.invokeAction("PublishMcaReports", collectionYear, period);
    }

    closePeriodEnd(collectionYear, period, collectionType) {
        this.jobController.setClosePeriodEndButtonState(false);
        this.invokeActionWithType("ClosePeriodEnd", collectionYear, period, collectionType);
    }

    resubmitJob(jobId) {
        document.getElementById("retryJob_" + jobId).style.visibility = "hidden";
        this.connection
            .invoke("ReSubmitJob", jobId)
            .catch((err) => {
                console.error(err.toString());
                document.getElementById("retryJob_" + jobId)[0].style.visibility = "visible";
            });
        return false;
    }

    collectionClosedEmail(collectionYear, period, collectionType) {
        this.jobController.setCollectionClosedEmailButtonState(false);
        this.invokeActionWithType("SendCollectionClosedEmail", collectionYear, period, collectionType);
    }

    pauseReferenceDataJobs(collectionYear, period) {
        this.jobController.setPauseRefJobsButtonState(false);
        this.invokeAction("PauseReferenceDataJobs", collectionYear, period);
    }

    proceed(collectionYear, period, pathId, pathItemId) {
        this.connection
            .invoke("Proceed", collectionYear, period, pathId, pathItemId)
            .catch(err => console.error(err.toString()));
        return false;
    }

    invokeAction(action, collectionYear, period) {
        this.connection
            .invoke(action, collectionYear, period)
            .catch(err => console.error(err.toString()));
    }

    invokeActionWithType(action, collectionYear, period, collectionType) {
        this.connection
            .invoke(action, collectionYear, period, collectionType)
            .catch(err => console.error(err.toString()));
    }
}

export default client;