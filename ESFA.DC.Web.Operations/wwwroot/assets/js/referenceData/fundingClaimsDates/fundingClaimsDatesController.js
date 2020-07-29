﻿import { getHandleBarsTemplate, Templates } from '/assets/js/handlebars-helpers.js';

class FundingClaimsDatesController {

    constructor() {
        this._fundingClaimsDatesList = {};
        this._userName = null;
        this._yearSelection = document.getElementById('collectionYears');
        this._yearSelected = null;
        this._spinner = document.getElementById('spinner');
    }

    init(userName, fundingClaimsDatesListModel) {
        this._userName = userName;
        this._yearSelected = this._yearSelection.value;
        this._yearSelection.addEventListener("change", this.yearsSelectionChange.bind(this));
        this.populateFundingClaimsDates(fundingClaimsDatesListModel);
    }

    yearsSelectionChange(e) {
        this._spinner.style.visibility = 'visible';
        this._yearSelected = this._yearSelection.value;
        window.fundingClaimsDatesClient.getFundingClaimsCollectionMetaDataByYear(this._yearSelected, this.populateFundingClaimsDates.bind(this));
        
    }

    populateFundingClaimsDates(fundingClaimsDatesList) {
        this._fundingClaimsDatesList = fundingClaimsDatesList;
        this.render(fundingClaimsDatesList);
    }

    render(fundingClaimsDatesList) {
        var compiledTemplate = getHandleBarsTemplate(Templates.FundingClaimsDatesList);
        var collections = fundingClaimsDatesList.map(function (elem) {
            return {
                value: elem.collectionId,
                text: elem.collectionName
            }
        });
        document.getElementById("fundingClaimsDatesList").innerHTML = compiledTemplate({ viewModel: fundingClaimsDatesList, collectionPeriods: collections });
        this.bindEvents();
        this._spinner.style.visibility = 'hidden';
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
        let fundingClaimsDates = this._fundingClaimsDatesList.find(x => x.id == id);
        fundingClaimsDates.inEditMode = false;
        this.render(this._fundingClaimsDatesList);
    }

    save(event) {
        event.preventDefault();
        this._spinner.style.visibility = 'visible';
        let form = event.target.closest('form');
        //let checkValidity = form.checkValidity();
        //form.reportValidity();
        var fundingClaimsCollectionMetadata = this.formToJson(form);
        fundingClaimsCollectionMetadata.createdBy = this._userName;
        window.fundingClaimsDatesClient.updateFundingClaimsCollectionMetadata(fundingClaimsCollectionMetadata, this.refreshFundingClaimsDates.bind(this));
    }

    refreshFundingClaimsDates() {
        window.fundingClaimsDatesClient.getFundingClaimsCollectionMetaDataByYear(this._yearSelected, this.populateFundingClaimsDates.bind(this));
    }

    formToJson(formElement) {
        let json = {};
        Array.from(formElement.querySelectorAll('input, select, textarea'))
            .filter(element => element.name)
            .forEach(element => {
                json[element.name] = element.type === 'checkbox' ? element.checked : element.value;
            });
        return json;
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
        }

        //const allinputs = document.querySelectorAll('input');

        //allinputs.blur(function (event) {
        //    event.target.checkValidity();
        //}).bind('invalid', function (event) {
        //    setTimeout(function () { $(event.target).focus(); }, 50);
        //});
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }
}

export default FundingClaimsDatesController