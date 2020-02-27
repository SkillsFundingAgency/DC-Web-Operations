import JobController from '/assets/js/periodEnd/jobController.js';

class client {
    
    constructor(connection) {
        this.connection = connection;
        this.jobController = new JobController();
    }

    startPeriodEnd(collectionYear, period) {
        this.jobController.setStartPeriodEndButtonState(false);
        invokeAction("StartPeriodEnd", collectionYear, period);
    }

    unPauseReferenceJobs(collectionYear, period) {
        this.jobController.setUnPauseReferenceJobsButtonState(false);
        invokeAction("UnPauseReferenceJobs", collectionYear, period);
    }

    publishProviderReports(collectionYear, period) {
        this.jobController.setPublishProviderReportsButtonState(false);
        invokeAction("PublishProviderReports", collectionYear, period);
    }

    publishMcaReports(collectionYear, period) {
        this.jobController.setPublishMcaReportsButtonState(false);
        invokeAction("PublishMcaReports", collectionYear, period);
    }

    closePeriodEnd(collectionYear, period) {
        this.jobController.setClosePeriodEndButtonState(false);
        invokeAction("ClosePeriodEnd", collectionYear, period);
    }

    invokeAction(action, collectionYear, period) {
        this.connection
            .invoke(action, collectionYear, period)
            .catch(err => console.error(err.toString()));
    }

    resubmitJob(jobId) {
        this.connection
            .invoke("ReSubmitJob", jobId)
            .catch(err => console.error(err.toString()));
    }

    collectionClosedEmail(collectionYear, period) {
        this.jobController.setCollectionClosedEmailButtonState(false);
        invokeAction("SendCollectionClosedEmail", collectionYear, period);
    }

    pauseReferenceDataJobs(collectionYear, period) {
        this.jobController.setPauseRefJobsButtonState(false);
        invokeAction("PauseReferenceDataJobs", collectionYear, period);
    }

    proceed(collectionYear, period, pathId, pathItemId) {
        this.connection
            .invoke("Proceed", collectionYear, period, pathId, pathItemId)
            .catch(err => console.error(err.toString()));
        return false;
    }
}

export default client;