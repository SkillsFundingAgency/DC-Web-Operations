import PathController from '/assets/js/periodEnd/allf/pathController.js';

class client {
    
    constructor(connection) {
        this.pathController = new PathController();
        this.connection = connection;
    }

    startPeriodEnd(collectionYear, period, collectionType) {
        this.invokeActionWithType("StartPeriodEnd", collectionYear, period, collectionType);
    }

    closePeriodEnd(collectionYear, period, collectionType) {
        this.invokeActionWithType("ClosePeriodEnd", collectionYear, period, collectionType);
    }

    resubmitJob(jobId) {
        this.connection
            .invoke("ReSubmitJob", jobId)
            .catch(err => console.error(err.toString()));
        return false;
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