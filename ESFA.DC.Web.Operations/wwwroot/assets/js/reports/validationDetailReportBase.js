import { setControlVisiblity } from '/assets/js/util.js';

export function setupValidationRulesAutocomplete(rules) {
    accessibleAutocomplete({
        element: document.querySelector('#tt-overlay'),
        id: 'autocomplete-overlay',
        displayMenu: 'overlay',
        confirmOnBlur: false,
        showNoOptionsFound: false,
        source: customRulesSuggest.bind(this, rules),
        onConfirm: searchRulesOnConfirm.bind(this),
        templates: {
            inputValue: searchRuleInputValueTemplate.bind(this),
            suggestion: searchRuleSuggestionTemplate.bind(this)
        },
        placeholder: 'e.g Rule_01'
    });
    var autocompleteElement = document.getElementById('autocomplete-overlay');
    autocompleteElement.addEventListener("blur", onautocompleteblur.bind(this));
    autocompleteElement.maxLength = "100";
    setControlVisiblity(false, 'spinner');
}

export function searchRuleInputValueTemplate(result) {
    return result && result.ruleName;
}

export function searchRuleSuggestionTemplate(result) {
    if (result.isTriggered === true) {
        return result.ruleName + '<strong> - [Triggered]</strong>';
    }
    else {
        return result.ruleName;
    }
}

export function customRulesSuggest(rules, query, syncResults) {
   syncResults(query
       ? rules.filter(function (result) {
            var resultContains = result.ruleName.toLowerCase().indexOf(query.toLowerCase()) !== -1;
            return resultContains;
        })
        : []
    );
}

export function onautocompleteblur() {
    if (document.getElementById('autocomplete-overlay')) {
        if (document.getElementById('autocomplete-overlay').value) {
            document.getElementById("generateValidationReport").disabled = false;
        } else {
            document.getElementById("generateValidationReport").disabled = true;
        }
    }
}

export function searchRulesOnConfirm(result) {
    document.getElementById("generateValidationReport").disabled = false;
}