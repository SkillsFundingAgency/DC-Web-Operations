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
        this._periodSelected = null;
        this._ruleSelected = null;
        this._validationReportGenerationUrl = null;
        this._reportGenerationUrl = null;
        this._reportsUrl = null;
        this._reportsDownloadUrl = null;
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
        this._changePeriodBtn.addEventListener("click", this.changePeriod.bind(this));
        this._reportSelection.addEventListener("change", this.onReportSelection.bind(this));
        this.hideValidationRuleDetailReportSection();
        this._validationReportGenerationUrl = validationReportGenerationUrl;
        this._reportGenerationUrl = reportGenerationUrl;
        this._reportsUrl = reportsUrl;
        this._reportsDownloadUrl = reportsDownloadUrl;
    }

    getReports() {
        this._yearSelected = document.getElementById('collectionYears').value;
        this._periodSelected = document.getElementById('collectionPeriod').value;
        window.reportClient.getReports(this._yearSelected, this._periodSelected, this.populateReports.bind(this));
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
        this._yearSelected = this._yearSelection.value;
        this._periodSelected = this._periodSelection.value;
        if (reportSelected === 'RuleValidationDetailReport') {
            this.removeElementsByClass('autocomplete__wrapper');
            this._spinner.style.visibility = 'visible';
            this._generateValidationReportButton.disabled = true;
            if (this._rulesByYear[this._yearSelected] === undefined) {
                window.reportClient.getValidationRules(this._yearSelected, this.populateRules.bind(this));
            } else {
                this.populateRules(this._rulesByYear[this._yearSelected]);
            }
        } else {
            window.reportClient.getReports(this._yearSelected,this._periodSelected, this.populateReports.bind(this));
        }
    }

    periodSelectionChange(e) {
        this._yearSelected = this._yearSelection.value;
        this._periodSelected = this._periodSelection.value;
        window.reportClient.getReports(this._yearSelected, this._periodSelected, this.populateReports.bind(this));
    }

    removeElementsByClass(className) {
        var elements = document.getElementsByClassName(className);
        while (elements.length > 0) {
            elements[0].parentNode.removeChild(elements[0]);
        }
    }

    populateReports(reportDetails) {
        var tableRef = document.getElementById('internalReportsTable').getElementsByTagName('tbody')[0];

        // remove the existing rows
        var elmtTable = document.getElementById('internalReportsTableBody');
        var tableRows = elmtTable.getElementsByTagName('tr');
        var rowCount = tableRows.length;
        for (var x = rowCount - 1; x >= 0; x--) {
            elmtTable.removeChild(tableRows[x]);
        }

        // display the rows
        for (var i = 0; i < reportDetails.length; i++) {
            var url = this._reportsDownloadUrl + '?collectionYear=' + this._yearSelected + '&collectionPeriod=' + this._periodSelected + '&fileName=' + reportDetails[i].url;
            var encodedUrl = encodeURI(url);
            var reportUrl = '<a href=' + encodedUrl + '> ' + reportDetails[i].url + '</a>';

            var htmlContent =
                '<tr class="govuk-table__row internalreports"><td class="govuk-table__cell" >' + reportDetails[i].displayName + '</td ><td class="govuk-table__cell">' + reportUrl + '</td></tr >';
            var newRow = tableRef.insertRow(tableRef.rows.length);
            newRow.innerHTML = htmlContent;
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