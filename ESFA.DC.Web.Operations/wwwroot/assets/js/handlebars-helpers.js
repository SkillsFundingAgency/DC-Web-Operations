import { removeSpaces } from './util.js';

export const getHandleBarsTemplate = function (name, root) {
    if (root === undefined) {
        root = '/assets/templates/';
    }

    if (Handlebars.templates === undefined || Handlebars.templates[name] === undefined) {
        var xhr = new XMLHttpRequest();

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    if (Handlebars.templates === undefined) {
                        Handlebars.templates = {};
                    }
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

export let Templates = {
    ReferenceDataFilesList: 'ReferenceData/FilesListTemplate.html',
    FundingClaimsDatesList: 'ReferenceData/FundingClaimsDatesList.html',
    InternalReportsDownloadList: 'Reports/InternalReportsDownloadList.html',
    ReportListOptions: 'Reports/ReportListOptions.html'
};

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

Handlebars.registerHelper('removeSpaces', function (str) {
    return removeSpaces(str);
});

Handlebars.registerHelper("setVar", function (varName, varValue, options) {
    if (!options.data.root) {
        options.data.root = {};
    }
    options.data.root[varName] = varValue;
});
