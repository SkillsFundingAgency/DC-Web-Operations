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
}

export default client;