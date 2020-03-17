class client {
    
    constructor(connection) {
        this.connection = connection;
    }

    getJobProcessingDetailsForCurrentPeriod(updatePage) {
        this.connection
            .invoke("GetJobProcessingDetailsForCurrentPeriod")
            .then(function (values) {
                updatePage(values);
            })
            .catch(err => console.error(err.toString()));
    }

    getJobProcessingDetailsForLastHour(updatePage) {
        this.connection
            .invoke("GetJobProcessingDetailsForLastHour")
            .then(function (values) {
                updatePage(values);
            })
            .catch(err => console.error(err.toString()));
    }

    getJobProcessingDetailsForLastFiveMins(updatePage) {
        this.connection
            .invoke("GetJobProcessingDetailsForLastFiveMins")
            .then(function (values) {
                updatePage(values);
            })
            .catch(err => console.error(err.toString()));
    }
}

export default client;