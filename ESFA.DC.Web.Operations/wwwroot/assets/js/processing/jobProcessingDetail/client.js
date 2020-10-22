class client {
    
    constructor(connection) {
        this.connection = connection;
    }

    getJobProcessingDetailsForCurrentPeriod(updatePage) {
        this.connection
            .invoke("GetJobProcessingDetailsForCurrentPeriod")
            .then(function (pageData) {
                updatePage({ pageData });
            })
            .catch(err => console.error(err.toString()));
    }

    getJobProcessingDetailsForLastHour(updatePage) {
        this.connection
            .invoke("GetJobProcessingDetailsForLastHour")
            .then(function (pageData) {
                updatePage({ pageData });
            })
            .catch(err => console.error(err.toString()));
    }

    getJobProcessingDetailsForLastFiveMins(updatePage) {
        this.connection
            .invoke("GetJobProcessingDetailsForLastFiveMins")
            .then(function (pageData) {
                updatePage({ pageData });
            })
            .catch(err => console.error(err.toString()));
    }
}

export default client;