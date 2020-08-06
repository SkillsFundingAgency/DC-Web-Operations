class client {

    constructor(connection) {
        this.connection = connection;
    }

    getFundingClaimsCollectionMetaDataByYear(year, populateFundingClaimsDates) {
        this.connection
            .invoke("GetFundingClaimsCollectionMetaDataByYear", year)
            .then(function(values) {
                populateFundingClaimsDates(values);
            })
            .catch(err => {
                alert(err.toString());
                console.error(err.toString());
            });
    }

    getFundingClaimsCollectionMetaData(populateFundingClaimsDates) {
        this.connection
            .invoke("GetFundingClaimsCollectionMetaData")
            .then(function (values) {
                populateFundingClaimsDates(values);
            })
            .catch(err => console.error(err.toString()));
    }

    updateFundingClaimsCollectionMetadata(data, refreshFundingClaimsDates) {
        this.connection
            .invoke("UpdateFundingClaimsCollectionMetaData", data)
            .then(function () {
                refreshFundingClaimsDates();
            })
            .catch(err => {
                    alert(err.toString());
                    console.error(err.toString());

                }
            );
    }
}

export default client;