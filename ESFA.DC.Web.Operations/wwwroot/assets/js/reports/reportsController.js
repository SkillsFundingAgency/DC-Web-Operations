import { getHandleBarsTemplate, Templates } from '/assets/js/handlebars-helpers.js';

class ReportsController {

    constructor() {
        this._reportSelection = document.getElementById('reportSelection');
        this._yearSelection = document.getElementById('collectionYears');
        this._periodSelection = document.getElementById('collectionPeriod');
        this._ruleValidationDetailReportSection = document.getElementById('ruleValidationDetailReportSection');
        this._createReportBtn = document.getElementById('createReport');
        this._generateValidationReportButton = document.getElementById("generateValidationReport");
        this._element = document.querySelector('#tt-overlay');
        this._id = 'autocomplete-overlay';
        this._spinner = document.getElementById('spinner');
        this._reportsLoadingSpinner = document.getElementById('reportsLoadingSpinner');
        this._rulesByYear = {};
        this._yearSelected = null;
        this._periodSelected = null;
        this._currentYear = null;
        this._currentPeriod = null;
        this._validationReportGenerationUrl = null;
        this._reportGenerationUrl = null;
        this._reportsUrl = null;
        this._reportsDownloadUrl = null;
        this.ValidationDetailReport = "RuleValidationDetailReport";
        this._internalReportsDownloadListDiv = document.getElementById("internalReportsDownloadList");
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    init(validationReportGenerationUrl, reportGenerationUrl, reportsUrl, reportsDownloadUrl) {
        this._yearSelection.addEventListener("change", this.yearsSelectionChange.bind(this));
        this._periodSelection.addEventListener("change", this.periodSelectionChange.bind(this));
        this._generateValidationReportButton.addEventListener("click", this.generateValidationDetailReport.bind(this));
        this._createReportBtn.addEventListener("click", this.createReport.bind(this));
        this._reportSelection.addEventListener("change", this.onReportSelection.bind(this));
        this.hideValidationRuleDetailReportSection();
        this._validationReportGenerationUrl = validationReportGenerationUrl;
        this._reportGenerationUrl = reportGenerationUrl;
        this._reportsUrl = reportsUrl;
        this._reportsDownloadUrl = reportsDownloadUrl;
    }

    getReports() {
        this._reportsLoadingSpinner.style.visibility = 'visible';
        this._yearSelected = document.getElementById('collectionYears').value;
        this._periodSelected = document.getElementById('collectionPeriod').value;

        window.reportClient.getReportDetails(this._yearSelected, this._periodSelected, this.populateReports.bind(this));

        if (this._reportSelection.value === this.ValidationDetailReport) {
            this._reportSelection.dispatchEvent(new Event('change'));
        }
    }

    hideValidationRuleDetailReportSection() {
        this._ruleValidationDetailReportSection.style.display = 'none';
        this._ruleValidationDetailReportSection.style.visibility = 'hidden';
        this._createReportBtn.style.visibility = 'visible';
        this._generateValidationReportButton.disabled = true;
        this.removeElementsByClass('autocomplete__wrapper');
    }

    showValidationRuleDetailReportSection() {
        this._yearSelected = this._yearSelection.value;
        this._spinner.style.visibility = 'visible';
        this._createReportBtn.style.visibility = 'hidden';
        this.removeElementsByClass('autocomplete__wrapper');
        window.reportClient.getValidationRules(this._yearSelected, this.populateRules.bind(this));
        this._ruleValidationDetailReportSection.style.visibility = 'visible';
        this._ruleValidationDetailReportSection.style.display = 'block';
    }

    onReportSelection(e) {
        var reportSelected = this._reportSelection.value;
        if (reportSelected === this.ValidationDetailReport) {
            this.showValidationRuleDetailReportSection();
        } else {
            this.hideValidationRuleDetailReportSection();
        }
    }

    yearsSelectionChange(e) {
        this._yearSelected = this._yearSelection.value;
        this._periodSelected = this._periodSelection.value;
        this.getReports();
        this.hideValidationRuleDetailReportSection();
    }

    periodSelectionChange(e) {
        this.getReports();
    }

    removeElementsByClass(className) {
        var elements = document.getElementsByClassName(className);
        while (elements.length > 0) {
            elements[0].parentNode.removeChild(elements[0]);
        }
    }

    populateReports(reportDetails) {
        var compileReportOptionsTemplate = getHandleBarsTemplate(Templates.ReportListOptions);
        this._reportSelection.innerHTML = compileReportOptionsTemplate({ viewModel: reportDetails });

        var compiledReportDownloadListTemplate = getHandleBarsTemplate(Templates.InternalReportsDownloadList);
        this._internalReportsDownloadListDiv.innerHTML = compiledReportDownloadListTemplate({ viewModel: reportDetails, yearSelected: this._yearSelected, periodSelected: this._periodSelected, downloadUrl: this._reportsDownloadUrl });

        if (this._reportSelection.value === this.ValidationDetailReport) {
            this._reportSelection.dispatchEvent(new Event('change'));
        }
        this._reportsLoadingSpinner.style.visibility = 'hidden';
    }

    populateRules(rules) {
        this.removeElementsByClass('autocomplete__wrapper');
        this._rulesByYear[this._yearSelected] = rules;
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
        this._spinner.style.visibility = 'hidden';
    }

    customRulesSuggest(query, syncResults) {
        var results = this._rulesByYear[this._yearSelected];
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
        var year = this._yearSelection.value;
        var periodValue = this._periodSelection.value;
        var rule = document.getElementById('autocomplete-overlay').value;
        if (rule) {
            this._spinner.style.visibility = 'visible';
            window.location.href = `${this._validationReportGenerationUrl}?year=${year}&Period=${periodValue}&rule=${rule}`;
        }
    }

    createReport(event) {
        event.preventDefault();
        var reportValue = this._reportSelection.value;
        var yearValue = this._yearSelection.value;
        var periodValue = this._periodSelection.value;
        window.location.href = this._reportGenerationUrl + '?reportName=' + reportValue + '&Year=' + yearValue + '&Period=' + periodValue;
    }

    changePeriod() {
        var yearValue = this._yearSelection.value;
        var periodValue = this._periodSelection.value;
        window.location.href = this._reportsUrl + '?collectionYear=' + yearValue + '&collectionPeriod=' + periodValue;
    }
}

export default ReportsController