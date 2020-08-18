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
        filename = options.hash.url;

    var url = reportsDownloadUrl + '?collectionYear=' + collectionYear + '&collectionPeriod=' + collectionPeriod + '&fileName=' + filename;
    var encodedUrl = encodeURI(url);

    var reportUrl = '<a href=' + encodedUrl + '> ' + filename + '</a>';
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