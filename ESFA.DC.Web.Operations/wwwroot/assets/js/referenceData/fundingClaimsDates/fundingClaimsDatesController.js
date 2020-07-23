class FundingClaimsDatesController {
    
    constructor() {
        
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    init() {
        console.log("init fundingclaimsdatescontroller");
    }

    getFundingClaimsDates() {
        //this._reportsLoadingSpinner.style.visibility = 'visible';
        //this._yearSelected = document.getElementById('collectionYears').value;
        //this._periodSelected = document.getElementById('collectionPeriod').value;

        //this._currentYear = document.getElementById('currentyear').value;;
        //this._currentPeriod = document.getElementById('currentperiod').value;;
        //if (this._currentYear == this._yearSelected && this._currentPeriod == this._periodSelected) {
        //    this._generateValidationReportButton.disabled = false;
        //    this._createReportButton.disabled = false;
        //} else {
        //    this._generateValidationReportButton.disabled = true;
        //    this._createReportButton.disabled = true;
        //}
        console.log("init getFundingClaimsDates");
        window.fundingClaimsDatesClient.getFundingClaimsCollectionMetaData(this.populateFundingClaimsDates.bind(this));

        //if (this._reportSelection.length === 1 && this._reportSelection.value === this.ValidationDetailReport) {
        //    this._reportSelection.dispatchEvent(new Event('change'));
        //}
    }

    populateFundingClaimsDates(reportDetails) {
        var reportDetail11 = reportDetails;
        console.log("populateFundingClaimsDates getFundingClaimsDates");
    }
}

export default FundingClaimsDatesController