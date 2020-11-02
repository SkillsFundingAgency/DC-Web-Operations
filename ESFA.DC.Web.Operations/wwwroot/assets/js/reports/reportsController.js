import { getHandleBarsTemplate, Templates } from '/assets/js/handlebars-helpers.js';
import { getInitialStateModel, parseToObject, $on } from '/assets/js/util.js';
import { setupValidationRulesAutocomplete } from '/assets/js/reports/validationDetailReportBase.js';
import Client from '/assets/js/reports/client.js';
import Hub from '/assets/js/hubs/hub.js';

class ReportsController {

    constructor({ initialState = getInitialStateModel() } = {}) {
        this._reportSelection = document.getElementById('reportSelection');
        this._yearSelection = document.getElementById('collectionYears');
        this._periodSelection = document.getElementById('collectionPeriod');
        this._ruleValidationDetailReportSection = document.getElementById('ruleValidationDetailReportSection');
        this._createReportBtn = document.getElementById('createReport');
        this._generateValidationReportButton = document.getElementById("generateValidationReport");
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
        this._data = parseToObject(initialState);
        var latestYear = this.setInitialYear();
        this.setPeriods(latestYear);

        $on(window, 'pageshow', () => {
            const hub = new Hub('reportsHub', this.displayConnectionState);
            hub.startHub(() => this.getReports());
            window.reportClient = new Client(hub.getConnection());
            this.init(this._data.validationReportGenerationUrl, this._data.reportGenerationUrl, this._data.reportsUrl, this._data.reportsDownloadUrl);
        });
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
        this.setPeriods(this._yearSelected);
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
        reportDetails.availableReportsList.sort((a, b) =>(a.displayName > b.displayName) ? 1 : ((b.displayName > a.displayName) ? -1 : 0));

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
        setupValidationRulesAutocomplete(rules);
    }

    generateValidationDetailReport() {
        let rule = document.getElementById('autocomplete-overlay').value;
        if (rule) {
            this._spinner.style.visibility = 'visible';
            window.location.href = `${this._validationReportGenerationUrl}?year=${this._yearSelected}&Period=${this._periodSelected}&rule=${rule}`;
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

    setInitialYear() {
        var latestCollectionYear = Object.entries(this._data.periods).reduce((a, b) => a[1] < b[1] ? a : b)[0];
        var collectionYear = document.getElementById('collectionYears');
        collectionYear.value = latestCollectionYear;
        return latestCollectionYear;
    }

    setPeriods(collectionYear) {
        var collectionPeriod = document.getElementById('collectionPeriod');
        collectionPeriod.options.length = 0;

        var collectionYearPeriods = this._data.periods[collectionYear];

        collectionYearPeriods.forEach((item) =>
            collectionPeriod.options[collectionPeriod.options.length] = new Option(item["text"], item["value"]));

        var latestPeriod = collectionYearPeriods.reduce((a, b) => parseInt(a.value,10) > parseInt(b.value,10) ? a : b);
        collectionPeriod.value = latestPeriod.value;
    }
}

export const reportsController = new ReportsController();
