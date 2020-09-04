import JobController from '/assets/js/periodEnd/ncs/jobController.js';

class client {
    
    constructor(connection) {
        this.connection = connection;
        this.jobController = new JobController();
    }

    startPeriodEnd(collectionYear, period) {
        this.jobController.setStartPeriodEndButtonState(false);
        this.invokeActionWithType("StartPeriodEnd", collectionYear, period);
    }

    closePeriodEnd(collectionYear, period) {
        this.jobController.setClosePeriodEndButtonState(false);
        this.invokeActionWithType("ClosePeriodEnd", collectionYear, period);
    }

    resubmitJob(jobId) {
        this.connection
            .invoke("ReSubmitJob", jobId)
            .catch(err => console.error(err.toString()));
        return false;
    }

    collectionClosedEmail(collectionYear, period) {
        this.jobController.setCollectionClosedEmailButtonState(false);
        this.invokeActionWithType("SendCollectionClosedEmail", collectionYear, period);
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

    invokeActionWithType(action, collectionYear, period) {
        this.connection
            .invoke(action, collectionYear, period)
            .catch(err => console.error(err.toString()));
    }
}

export default client;