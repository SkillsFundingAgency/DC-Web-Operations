export const getHandleBarsTemplate = function (name) {
    return Handlebars.templates[name];
};

export const registerPartialTemplate = function (name, template) {
    const partialTemplate = getHandleBarsTemplate(template);
    Handlebars.registerPartial(name, partialTemplate);
}

export const Templates = {
    ReferenceDataFilesList: 'ReferenceData/FilesListTemplate',
    FundingClaimsDatesList: 'ReferenceData/FundingClaimsDatesList',
    InternalReportsDownloadList: 'Reports/InternalReportsDownloadList',
    ReportListOptions: 'Reports/ReportListOptions',
    PeriodEnd: 'PeriodEnd/PeriodEnd',
    PeriodEndNavigation: 'PeriodEnd/PeriodEndNavigation',
    FisFilesList: 'ReferenceData/FisFilesListTemplate',
    ALLFPeriodEndFileList: 'PeriodEnd/ALLFPeriodEndFileList'
};

export const Partials = {
    ProceedButton: 'PeriodEnd/Partials/ProceedButton',
    PathItemJobSummary: 'PeriodEnd/Partials/PathItemJobSummary',
    ProceedableItemWrapper: 'PeriodEnd/Partials/ProceedableItemWrapper',
    PathHeader: 'PeriodEnd/Partials/PathHeader',
    ILRPathHeader: 'PeriodEnd/Partials/ILRPathHeader'
}

export const registerHelper = function (helper, helperFunction) {
    Handlebars.registerHelper(helper, helperFunction);
}

export const registerHelpers = function(helpers) {
    for (let [helper, helperFunction] of Object.entries(helpers)) {
        registerHelper(helper, helperFunction);
    }
}

Handlebars.registerHelper('jobStatusClass', function (displayStatus) {
    var statusClass = displayStatus === 'Job Completed' ? 'jobCompleted'
        : displayStatus === 'Job Rejected' ? 'jobRejected'
            : displayStatus === 'Job Failed' ? 'jobFailed'
                : '';
    return statusClass;
});

Handlebars.registerHelper('encodedReportUrl', function (context, options) {
    var collectionYear = options.hash.yearSelected,
        collectionPeriod = options.hash.periodSelected,
        reportsDownloadUrl = options.hash.downloadUrl,
        reportDisplayName = options.hash.reportDisplayName,
        filename = options.hash.url;

    var url = reportsDownloadUrl + '?collectionYear=' + collectionYear + '&collectionPeriod=' + collectionPeriod + '&fileName=' + filename + '&reportDisplayName=' + reportDisplayName;
    var encodedUrl = encodeURI(url);

    return encodedUrl;
});

Handlebars.registerHelper('select', function (value, options) {
    return options.fn()
        .split('\n')
        .map(function (v) {
            var t = 'value="' + value + '"';
            return RegExp(t).test(v) ? v.replace(t, t + ' selected="selected"') : v;
        })
        .join('\n');
});

Handlebars.registerHelper("setVar", function (varName, varValue, options) {
    if (!options.data.root) {
        options.data.root = {};
    }
    options.data.root[varName] = varValue;
});


