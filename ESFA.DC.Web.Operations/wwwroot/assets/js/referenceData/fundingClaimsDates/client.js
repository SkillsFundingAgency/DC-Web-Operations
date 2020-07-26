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

    updateFundingClaimsCollectionMetadata(data) {
        this.connection
            .invoke("UpdateFundingClaimsCollectionMetaData", data)
            .then(function () {
                //populateFundingClaimsDates(values);
            })
            .catch(err => {
                    alert(err.toString());
                console.error(err.toString());
                
            }
            );
    }
}

export default client;