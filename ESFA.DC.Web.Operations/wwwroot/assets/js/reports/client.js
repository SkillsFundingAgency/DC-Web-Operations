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
}

export default client;