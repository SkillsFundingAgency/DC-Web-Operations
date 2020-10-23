class client {
    
    constructor(connection) {
        this.connection = connection;
    }

    getJobProcessingDetailsForCurrentPeriod(updatePage) {
        this.connection
            .invoke("GetJobProcessingDetailsForCurrentPeriod")
            .then(function (jobs) {
                updatePage({ jobs });
            })
            .catch(err => console.error(err.toString()));
    }

    getJobProcessingDetailsForLastHour(updatePage) {
        this.connection
            .invoke("GetJobProcessingDetailsForLastHour")
            .then(function (jobs) {
                updatePage({ jobs });
            })
            .catch(err => console.error(err.toString()));
    }

    getJobProcessingDetailsForLastFiveMins(updatePage) {
        this.connection
            .invoke("GetJobProcessingDetailsForLastFiveMins")
            .then(function (jobs) {
                updatePage({ jobs });
            })
            .catch(err => console.error(err.toString()));
    }
}

export default client;