export const getHandleBarsTemplate = function (name, root) {
    if (root === undefined) {
        root = '/assets/templates/';
    }

    if (Handlebars.templates === undefined || Handlebars.templates[name] === undefined) {

        var xhr = new XMLHttpRequest();

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    Handlebars.templates = Handlebars.templates || {};
                    Handlebars.templates[name] = Handlebars.compile(xhr.responseText);
                }
                else {
                    console.log('Error fetching the template.');
                }
            }
        }

        xhr.open("GET", root + name, false);
        xhr.send();
    }
    return Handlebars.templates[name];
};

export const registerPartialTemplate = function (name, template) {
    const partialTemplate = getHandleBarsTemplate(template);
    Handlebars.registerPartial(name, partialTemplate);
}

export const Templates = {
    ReferenceDataFilesList: 'ReferenceData/FilesListTemplate.html',
    FundingClaimsDatesList: 'ReferenceData/FundingClaimsDatesList.html',
    InternalReportsDownloadList: 'Reports/InternalReportsDownloadList.html',
    ReportListOptions: 'Reports/ReportListOptions.html',
    PeriodEnd: 'PeriodEnd/PeriodEnd.html',
    ILRPeriodEndNavigation: 'PeriodEnd/ILRPeriodEndNavigation.html',
    FisFilesList: 'ReferenceData/FisFilesListTemplate.html'
    ALLFPeriodEndFileList: 'PeriodEnd/ALLFPeriodEndFileList.html'
};

export const Partials = {
    ProceedButton: 'PeriodEnd/Partials/ProceedButton.html',
    PathItemJobSummary: 'PeriodEnd/Partials/PathItemJobSummary.html',
    ProceedableItemWrapper: 'PeriodEnd/Partials/ProceedableItemWrapper.html',
    PathHeader: 'PeriodEnd/Partials/PathHeader.html',
    ALLFPathHeader: 'PeriodEnd/Partials/ALLFPathHeader.html'
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


