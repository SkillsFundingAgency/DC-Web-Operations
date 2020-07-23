class client {
    
    constructor(connection) {
        this.connection = connection;
    }

    getFundingClaimsCollectionMetaData(populateFundingClaimsDates) {
        this.connection
            .invoke("GetFundingClaimsCollectionMetaData")
            .then(function (values) {
                populateFundingClaimsDates(values);
            })
            .catch(err => console.error(err.toString()));
    }
}

export default client;