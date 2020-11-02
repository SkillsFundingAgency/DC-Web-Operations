import { getInitialStateModel, parseToObject, $on, setControlVisiblity } from '/assets/js/util.js';
import { setupAutocomplete } from '/assets/js/reports/validationDetailReportBase.js';
import Client from '/assets/js/reports/client.js';
import Hub from '/assets/js/hubs/hub.js';

class ValidationDetailReportController {

    constructor({ initialState = getInitialStateModel() } = {}) {
        this._ruleValidationDetailReportSection = document.getElementById('ruleValidationDetailReportSection');
        this._generateValidationReportButton = document.getElementById("generateValidationReport");
        this._element = document.querySelector('#tt-overlay');
        this._id = 'autocomplete-overlay';
        this._spinner = document.getElementById('spinner');
        this._rules = {};
        this._data = parseToObject(initialState);
        $on(window, 'pageshow', () => {
            const hub = new Hub('reportsHub', this.displayConnectionState);
            hub.startHub(() => this.renderValidationRuleDetailByYear());
            window.reportClient = new Client(hub.getConnection());
        });
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    renderValidationRuleDetailByYear() {
        this._generateValidationReportButton.addEventListener("click", this.generateValidationDetailReport.bind(this));
        window.reportClient.getValidationRules(this._data.year, this.populateRules.bind(this));
    }

    populateRules(rules) {
        this._rules = rules;
        //setupAutocomplete(rules);
        accessibleAutocomplete({
            element: this._element,
            id: this._id,
            displayMenu: 'overlay',
            confirmOnBlur: false,
            showNoOptionsFound: false,
            source: this.customRulesSuggest.bind(this),
            onConfirm: this.searchRulesOnConfirm.bind(this),
            templates: {
                inputValue: this.searchRuleInputValueTemplate.bind(this),
                suggestion: this.searchRuleSuggestionTemplate.bind(this)
            },
            placeholder: 'e.g Rule_01'
        });
        var autocompleteElement = document.getElementById('autocomplete-overlay');
        autocompleteElement.addEventListener("blur", this.onautocompleteblur.bind(this));
        autocompleteElement.maxLength = "100";
        setControlVisiblity(false, 'spinner');
    }

    customRulesSuggest(query, syncResults) {
        var results = this._rules;
        syncResults(query
            ? results.filter(function (result) {
                var resultContains = result.ruleName.toLowerCase().indexOf(query.toLowerCase()) !== -1;
                return resultContains;
            })
            : []
        );
    }

    searchRuleInputValueTemplate(result) {
        return result && result.ruleName;
    }

    searchRuleSuggestionTemplate(result) {
        if (result.isTriggered === true) {
            return result.ruleName + '<strong> - [Triggered]</strong>';
        }
        else {
            return result.ruleName;
        }
    }

    onautocompleteblur() {
        if (this._id) {
            if (document.getElementById('autocomplete-overlay').value) {
                this._generateValidationReportButton.disabled = false;
            } else {
                this._generateValidationReportButton.disabled = true;
            }
        }
    }

    searchRulesOnConfirm(result) {
        this._generateValidationReportButton.disabled = false;
    }

    generateValidationDetailReport() {
        let rule = document.getElementById('autocomplete-overlay').value;
        if (rule) {
            setControlVisiblity(true, 'spinner');
            window.location.href = `${this._data.validationReportGenerationUrl}?year=${this._data.year}&Period=${this._data.period}&rule=${rule}`;
        }
    }
}

export const validationDetailReportController = new ValidationDetailReportController();
