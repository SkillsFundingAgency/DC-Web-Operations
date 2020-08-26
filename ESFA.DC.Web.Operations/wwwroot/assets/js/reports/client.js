class client {
    
    constructor(connection) {
        this.connection = connection;
    }

    getCollectionYears(populateCollections) {
        this.connection
            .invoke("GetCollectionYears")
            .then(function(values) {
                populateCollections(values);
            })
            .catch(err => console.error(err.toString()));
    }

    getValidationRules(year, populateResults) {
        this.connection
            .invoke("GetValidationRules", year)
            .then(function (values) {
                populateResults(values);
            })
            .catch(err => console.error(err.toString()));
    }


    getReportDetails(collectionYear, collectionPeriod, populateReports) {
        this.connection
            .invoke("GetReportDetails", collectionYear, collectionPeriod)
            .then(function (values) {
                populateReports(values);
            })
            .catch(err => console.error(err.toString()));
    }
}

export default client;