class ReportsController {

    constructor() {
        this._reportSelection = document.getElementById('reportSelection');
        this._yearSelection = document.getElementById('collectionYears');
        this._periodSelection = document.getElementById('collectionPeriod');
        this._ruleValidationDetailReportSection = document.getElementById('ruleValidationDetailReportSection');
        this._createReportBtn = document.getElementById('createReport');
        this._changePeriodBtn = document.getElementById('changePeriod');
        this._generateValidationReportButton = document.getElementById("generateValidationReport");
        this._element = document.querySelector('#tt-overlay');
        this._id = 'autocomplete-overlay';
        this._spinner = document.getElementById('spinner');
        this._collectionYears = {};
        this._rulesByYear = {};
        this._yearSelected = null;
        this._ruleSelected = null;
        this._validationReportGenerationUrl = null;
        this._reportGenerationUrl = null;
        this._reportsUrl = null;
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    init(validationReportGenerationUrl, reportGenerationUrl, reportsUrl) {
        this._yearSelection.addEventListener("change", this.yearsSelectionChange.bind(this));
        this._generateValidationReportButton.addEventListener("click", this.generateValidationDetailReport.bind(this));
        this._createReportBtn.addEventListener("click", this.createReport.bind(this));
        this._changePeriodBtn.addEventListener("click", this.changePeriod.bind(this));
        this._reportSelection.addEventListener("change", this.onReportSelection.bind(this));
        this.hideValidationRuleDetailReportSection();
        this._validationReportGenerationUrl = validationReportGenerationUrl;
        this._reportGenerationUrl = reportGenerationUrl;
        this._reportsUrl = reportsUrl;
    }

    hideValidationRuleDetailReportSection() {
        this._ruleValidationDetailReportSection.style.visibility = 'hidden';
        this._createReportBtn.style.visibility = 'visible';
        this._generateValidationReportButton.disabled = true;
    }

    showValidationRuleDetailReportSection() {
        if (Object.keys(this._collectionYears).length === 0) {
            this._yearSelected = this._yearSelection.value;
            this._spinner.style.visibility = 'visible';
            this._createReportBtn.style.visibility = 'hidden';
            this.removeElementsByClass('autocomplete__wrapper');
            if (this._rulesByYear[this._yearSelected] === undefined) {
                window.reportClient.getValidationRules(this._yearSelected, this.populateRules.bind(this));
            } else {
                this.populateRules(this._rulesByYear[this._yearSelected]);
            }
        }
        this._ruleValidationDetailReportSection.style.visibility = 'visible';
    }

    onReportSelection(e) {
        var reportSelected = this._reportSelection.value;
        if (reportSelected === 'RuleValidationDetailReport') {
            this.showValidationRuleDetailReportSection();
        } else {
            this.hideValidationRuleDetailReportSection();
        }
    }

    yearsSelectionChange(e) {
        var reportSelected = this._reportSelection.value;
        if (reportSelected === 'RuleValidationDetailReport') {
            this._yearSelected = this._yearSelection.value;
            this.removeElementsByClass('autocomplete__wrapper');
            this._spinner.style.visibility = 'visible';
            this._generateValidationReportButton.disabled = true;
            if (this._rulesByYear[this._yearSelected] === undefined) {
                window.reportClient.getValidationRules(this._yearSelected, this.populateRules.bind(this));
            } else {
                this.populateRules(this._rulesByYear[this._yearSelected]);
            }
        }
    }

    removeElementsByClass(className) {
        var elements = document.getElementsByClassName(className);
        while (elements.length > 0) {
            elements[0].parentNode.removeChild(elements[0]);
        }
    }

    populateRules(rules) {
        this._rulesByYear[this._yearSelected] = rules;
        accessibleAutocomplete({
            element: this._element,
            id: this._id,
            displayMenu: 'overlay',
            confirmOnBlur: false,
            showNoOptionsFound: false,
            source: rules,
            onConfirm: this.searchRulesOnConfirm.bind(this),
            placeholder: 'e.g Rule_01'
        });
        this._spinner.style.visibility = 'hidden';
    }

    searchRulesOnConfirm(result) {
        this._ruleSelected = result;
        this._generateValidationReportButton.disabled = false;
    }

    generateValidationDetailReport() {
        var year = this._yearSelection.value;
        var rule = document.getElementById('autocomplete-overlay').value;
        if (rule) {
            this._spinner.style.visibility = 'visible';
            window.location.href = this._validationReportGenerationUrl + '?year=' + year + '&rule=' + rule;
        }
    }

    createReport() {
        var reportValue = this._reportSelection.value;
        var yearValue = this._yearSelection.value;
        var periodValue = this._periodSelection.value;
        window.location.href = this._reportGenerationUrl + '?reportType=' + reportValue + '&Year=' + yearValue + '&Period=' + periodValue;
    }

    changePeriod() {
        var yearValue = this._yearSelection.value;
        var periodValue = this._periodSelection.value;
        var reportValue = this._reportSelection.value;
        window.location.href = this._reportsUrl + '?collectionYear=' + yearValue + '&collectionPeriod=' + periodValue;
    }

}

export default ReportsController