class client {
    
    constructor(connection) {
        this.connection = connection;
    }

    startPeriodEnd(collectionYear, period) {
        this.connection
            .invoke("StartPeriodEnd", collectionYear, period)
            .catch(err => console.error(err.toString()));
    }

    resubmitJob(collectionYear, period, jobId) {
        this.connection
            .invoke("ReSubmitJob", collectionYear, period, jobId)
            .catch(err => console.error(err.toString()));
    }
}

export default client;