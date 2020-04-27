import JobController from '/assets/js/periodEnd/ncs/jobController.js';

class client {
    
    constructor(connection) {
        this.connection = connection;
        this.jobController = new JobController();
    }

    startPeriodEnd(collectionYear, period, collectionType) {
        this.jobController.setStartPeriodEndButtonState(false);
        this.invokeActionWithType("StartPeriodEnd", collectionYear, period, collectionType);
    }

    closePeriodEnd(collectionYear, period, collectionType) {
        this.jobController.setClosePeriodEndButtonState(false);
        this.invokeActionWithType("ClosePeriodEnd", collectionYear, period, collectionType);
    }

    resubmitJob(jobId) {
        this.connection
            .invoke("ReSubmitJob", jobId)
            .catch(err => console.error(err.toString()));
        return false;
    }

    collectionClosedEmail(collectionYear, period, collectionType) {
        this.jobController.setCollectionClosedEmailButtonState(false);
        this.invokeActionWithType("SendCollectionClosedEmail", collectionYear, period, collectionType);
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