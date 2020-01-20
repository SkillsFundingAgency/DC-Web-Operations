import JobController from '/assets/js/periodEnd/jobController.js';

class client {
    
    constructor(connection) {
        this.connection = connection;
        this.jobController = new JobController();
    }

    startPeriodEnd(collectionYear, period) {
        this.connection
            .invoke("StartPeriodEnd", collectionYear, period)
            .catch(err => console.error(err.toString()));
    }

    resubmitJob(jobId) {
        this.connection
            .invoke("ReSubmitJob", jobId)
            .catch(err => console.error(err.toString()));
    }

    collectionClosedEmail(collectionYear, period) {
        this.jobController.setCollectionClosedEmailButtonState(false);

        this.connection
            .invoke("SendCollectionClosedEmail", collectionYear, period)
            .catch(err => console.error(err.toString()));
    }

    pauseReferenceDataJobs(collectionYear, period) {
        this.jobController.setPauseRefJobsButtonState(false);

        this.connection
            .invoke("PauseReferenceDataJobs", collectionYear, period)
            .catch(err => console.error(err.toString()));
    }

    proceed(collectionYear, period, pathId, pathItemId) {
        this.connection
            .invoke("Proceed", collectionYear, period, pathId, pathItemId)
            .catch(err => console.error(err.toString()));
    }
}

export default client;