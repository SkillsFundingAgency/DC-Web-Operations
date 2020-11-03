import { setControlVisiblity, getInitialStateModel } from '/assets/js/util.js';

class ValidationDetailReportBase {
    constructor({ initialState = getInitialStateModel() } = {}) {
    }

    setupValidationRulesAutocomplete(rules) {
        accessibleAutocomplete({
            element: document.querySelector('#tt-overlay'),
            id: 'autocomplete-overlay',
            displayMenu: 'overlay',
            confirmOnBlur: false,
            showNoOptionsFound: false,
            source: this.customRulesSuggest.bind(this, rules),
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

    customRulesSuggest(rules, query, syncResults) {
        syncResults(query
            ? rules.filter(function (result) {
                var resultContains = result.ruleName.toLowerCase().indexOf(query.toLowerCase()) !== -1;
                return resultContains;
            })
            : []
        );
    }

    onautocompleteblur() {
        if (document.getElementById('autocomplete-overlay')) {
            if (document.getElementById('autocomplete-overlay').value) {
                document.getElementById("generateValidationReport").disabled = false;
            } else {
                document.getElementById("generateValidationReport").disabled = true;
            }
        }
    }

    searchRulesOnConfirm(result) {
        document.getElementById("generateValidationReport").disabled = false;
    }
}
export default ValidationDetailReportBase