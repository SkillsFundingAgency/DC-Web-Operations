import { setControlEnabledState } from '/assets/js/util.js';

class client {
    
    constructor(connection) {
        this.connection = connection;
    }

    startPeriodEnd(collectionYear, period) {
        setControlEnabledState(false, "startPeriodEnd");
        this.invokeActionWithType("StartPeriodEnd", collectionYear, period);
    }

    unPauseReferenceJobs(collectionYear, period) {
        setControlEnabledState(false, "resumeReferenceData");
        this.invokeAction("UnPauseReferenceJobs", collectionYear, period);
    }

    publishProviderReports(collectionYear, period) {
        setControlEnabledState(false, "publishProviderReports");
        this.invokeAction("PublishProviderReports", collectionYear, period);
    }

    publishMcaReports(collectionYear, period) {
        setControlEnabledState(false, "publishMcaReports");
        this.invokeAction("PublishMcaReports", collectionYear, period);
    }

    closePeriodEnd(collectionYear, period) {
        setControlEnabledState(false, "closePeriodEnd");
        this.invokeActionWithType("ClosePeriodEnd", collectionYear, period);
    }

    resubmitJob(jobId) {
        document.getElementById("retryJob_" + jobId).style.visibility = "hidden";
        this.connection
            .invoke("ReSubmitJob", jobId)
            .catch((err) => {
                console.error(err.toString());
                document.getElementById("retryJob_" + jobId).style.visibility = "visible";
            });
        return false;
    }

    collectionClosedEmail(collectionYear, period) {
        setControlEnabledState(false, "collectionClosed");
        this.invokeActionWithType("SendCollectionClosedEmail", collectionYear, period);
    }

    pauseReferenceDataJobs(collectionYear, period) {
        setControlEnabledState(false, "pauseReferenceData");
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

    invokeActionWithType(action, collectionYear, period) {
        this.connection
            .invoke(action, collectionYear, period)
            .catch(err => console.error(err.toString()));
    }
}

export default client;