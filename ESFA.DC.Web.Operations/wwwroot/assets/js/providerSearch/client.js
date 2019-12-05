class client {
    
    constructor(connection) {
        this.connection = connection;
    }

    providerSearch(query, populateResults) {
        this.connection
            .invoke("ProviderSearch", query)
            .then(function(values) {
                populateResults(values);
            })
            .catch(err => console.error(err.toString()));
    }

    providerSearchExisting(query, populateResults) {
        this.connection
            .invoke("ProviderSearchExisting", query)
            .then(function (values) {
                populateResults(values);
            })
            .catch(err => console.error(err.toString()));
    }

}

export default client;