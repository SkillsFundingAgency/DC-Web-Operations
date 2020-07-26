import { getHandleBarsTemplate, Templates } from '/assets/js/handlebars-helpers.js';

class FundingClaimsDatesController {

    constructor() {
        this._fundingClaimsDatesList = {};
        this._userName = null;
    }

    init(userName) {
        console.log("init fundingClaimsDatesController");
        this._userName = userName;
    }

    bindEvents() {
       const modifyButtons = document.querySelectorAll('#modify');
        for (const modifyButton of modifyButtons) {
            modifyButton.addEventListener("click", this.modify.bind(this));
        }

        const cancelLinks = document.querySelectorAll('#cancel');
        for (const cancel of cancelLinks) {
            cancel.addEventListener("click", this.cancel.bind(this));
        }

        const saveButtons = document.querySelectorAll('#save');
        for (const saveBtn of saveButtons) {
            saveBtn.addEventListener("click", this.save.bind(this));
        }}

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    getFundingClaimsDates() {
        console.log("init getFundingClaimsDates");
        window.fundingClaimsDatesClient.getFundingClaimsCollectionMetaData(this.populateFundingClaimsDates.bind(this));
    }

    populateFundingClaimsDates(fundingClaimsDatesList) {
        this._fundingClaimsDatesList = fundingClaimsDatesList;
        this.render(fundingClaimsDatesList);
        console.log("populateFundingClaimsDates getFundingClaimsDates");
    }

    render(fundingClaimsDatesList) {
        var compiledTemplate = getHandleBarsTemplate(Templates.FundingClaimsDatesList);
        document.getElementById("fundingClaimsDatesList").innerHTML = compiledTemplate({ viewModel: fundingClaimsDatesList });
        this.bindEvents();
    }

    modify(event) {
        let id = event.target.dataset.id;
        let found = this._fundingClaimsDatesList.find(x => x.id == id);
        found.inEditMode = true;

        let others = this._fundingClaimsDatesList.filter(x => x.id !== found.id);
        for (const other of others) {
            other.inEditMode = false;
        }
        
        this.render(this._fundingClaimsDatesList);
    }

    cancel(event) {
        let id = event.target.dataset.id;
        let found = this._fundingClaimsDatesList.find(x => x.id == id);
        found.inEditMode = false;
        this.render(this._fundingClaimsDatesList);
    }

    save(event) {
        let form = event.target.closest('form');
        var fundingClaimsCollectionMetadata = this.formToJson(form);
        fundingClaimsCollectionMetadata.createdBy = this._userName;
        window.fundingClaimsDatesClient.updateFundingClaimsCollectionMetadata(fundingClaimsCollectionMetadata);
        this.render(this._fundingClaimsDatesList);

    }

    formToJson(formElement) {
        //var inputElements = formElement.getElementsByTagName("input , select"),
        //    jsonObject = {};
        //for (var i = 0; i < inputElements.length; i++) {
        //    var inputElement = inputElements[i];
        //    jsonObject[inputElement.name] = inputElement.value;

        //}
        //return JSON.stringify(jsonObject);

        let json = {};
        Array.from(formElement.querySelectorAll('input, select, textarea'))
            .filter(element => element.name)
            .forEach(element => {
                json[element.name] = element.type === 'checkbox' ? element.checked : element.value;
            });
        return json;
    }
}

export default FundingClaimsDatesController