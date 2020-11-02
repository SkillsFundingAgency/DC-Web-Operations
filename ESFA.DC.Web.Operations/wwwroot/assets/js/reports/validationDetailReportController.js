import { getInitialStateModel, parseToObject, $on, setControlVisiblity } from '/assets/js/util.js';
import { setupValidationRulesAutocomplete } from '/assets/js/reports/validationDetailReportBase.js';
import Client from '/assets/js/reports/client.js';
import Hub from '/assets/js/hubs/hub.js';

class ValidationDetailReportController {

    constructor({ initialState = getInitialStateModel() } = {}) {
        this._ruleValidationDetailReportSection = document.getElementById('ruleValidationDetailReportSection');
        this._generateValidationReportButton = document.getElementById("generateValidationReport");
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
        setupValidationRulesAutocomplete(rules);
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
