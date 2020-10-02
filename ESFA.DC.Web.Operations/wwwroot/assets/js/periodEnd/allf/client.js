import { setControlEnabledState } from '/assets/js/util.js';

class client {
    
    constructor(connection) {
        this.connection = connection;
    }

    startPeriodEnd(collectionYear, period) {
        setControlEnabledState(false, "startPeriodEnd");
        this.invokeActionWithType("StartPeriodEnd", collectionYear, period);
    }

    closePeriodEnd(collectionYear, period) {
        setControlEnabledState(false, "closePeriodEnd");
        setControlEnabledState(false, "uploadFile");
        this.invokeActionWithType("ClosePeriodEnd", collectionYear, period);
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

    invokeActionWithType(action, collectionYear, period) {
        this.connection
            .invoke(action, collectionYear, period)
            .catch(err => console.error(err.toString()));
    }
}

export default client;