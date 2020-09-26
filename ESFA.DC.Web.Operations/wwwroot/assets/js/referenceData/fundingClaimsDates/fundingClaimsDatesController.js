import { getHandleBarsTemplate, Templates } from '/assets/js/handlebars-helpers.js';

class FundingClaimsDatesController {

    constructor() {
        this._fundingClaimsDatesListDiv = document.getElementById("fundingClaimsDatesList");
        this._fundingClaimsDatesList = {};
        this._userName = null;
        this._yearSelection = document.getElementById('collectionYears');
        this._yearSelected = null;
        this._spinner = document.getElementById('spinner');
        this._updateMessageSpan = document.getElementById('update-message');
        this._errorSpan = null;
        this._collections = {};
        this._fundingClaimsDatesModel = {}

    }

    init(userName, fundingClaimsDatesModel) {
        this._userName = userName;
        this._yearSelected = this._yearSelection.value;
        this._yearSelection.addEventListener("change", this.yearsSelectionChange.bind(this));
        this.populateFundingClaimsDates(fundingClaimsDatesModel);
    }

    yearsSelectionChange(e) {
        this._spinner.style.visibility = 'visible';
        this._yearSelected = this._yearSelection.value;
        window.fundingClaimsDatesClient.getFundingClaimsCollectionMetaDataByYear(this._yearSelected, this.populateFundingClaimsDates.bind(this));

    }

    populateFundingClaimsDates(fundingClaimsDatesModel) {
        this._collections = fundingClaimsDatesModel.collections;
        this._fundingClaimsDatesList = fundingClaimsDatesModel.fundingClaimsDatesList;
        this.render(this._fundingClaimsDatesList);
    }

    render(fundingClaimsDatesList) {
        var compiledTemplate = getHandleBarsTemplate(Templates.FundingClaimsDatesList);
        var collectionPeriodsList = this._collections.map(function (elem) {
            return {
                value: elem.collectionId,
                text: elem.collectionTitle
            }
        });
        this._fundingClaimsDatesListDiv.innerHTML = compiledTemplate({ viewModel: fundingClaimsDatesList, collectionPeriods: collectionPeriodsList });
        this.bindEvents();
        this._spinner.style.visibility = 'hidden';
    }

    modify(event) {
        let collectionId = event.target.dataset.collectionid;
        let found = this._fundingClaimsDatesList.find(x => x.collectionId == collectionId);
        found.inEditMode = true;

        let others = this._fundingClaimsDatesList.filter(x => x.collectionId !== found.collectionId);
        for (const other of others) {
            other.inEditMode = false;
        }

        this.render(this._fundingClaimsDatesList);
    }

    cancel(event) {
        let collectionId = event.target.dataset.collectionid;
        let fundingClaimsDates = this._fundingClaimsDatesList.find(x => x.collectionId == collectionId);
        fundingClaimsDates.inEditMode = false;
        this.render(this._fundingClaimsDatesList);
    }

    save(event) {
        event.preventDefault();
        let form = event.target.closest('form');
        if (!this.IsFormValid()) {
            this._errorSpan.style.display = "block";
            return;
        }
        this._spinner.style.visibility = 'visible';
        var fundingClaimsCollectionMetadata = this.formToJson(form);
        fundingClaimsCollectionMetadata.updatedBy = this._userName;
        window.fundingClaimsDatesClient.updateFundingClaimsCollectionMetadata(fundingClaimsCollectionMetadata, this.refreshFundingClaimsDates.bind(this));
    }

    refreshFundingClaimsDates() {
        document.getElementById('update-message').style.display = "block";
        setTimeout(function () {
            document.getElementById('update-message').style.display = "none";
        }, 3000);
        window.fundingClaimsDatesClient.getFundingClaimsCollectionMetaDataByYear(this._yearSelected, this.populateFundingClaimsDates.bind(this));
    }

    IsFormValid() {
        const allinputs = document.querySelectorAll('input');
        for (const input of allinputs) {
            var isValidInput = input.checkValidity();
            if (!isValidInput) {
                return false;
            }
        }
        return true;
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

        // validation 
        const allinputs = document.querySelectorAll('input');

        for (const input of allinputs) {
            input.addEventListener("blur", this.validateInput.bind(this));
        }

        this._errorSpan = document.getElementById('fundingclaimsDates-error');
        this._errorSpan.style.display = "none";
    }

    validateInput(event) {
        this._errorSpan.style.display = "none";
        var isValid = event.target.checkValidity();
        if (!isValid) {
            event.target.classList.add("govuk-input--error");
        } else {
            event.target.classList.remove("govuk-input--error");
        }
    }


    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }
}

export default FundingClaimsDatesController