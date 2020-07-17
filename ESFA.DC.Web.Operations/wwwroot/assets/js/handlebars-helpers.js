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
    ReferenceDataFilesList: 'ReferenceData/FilesListTemplate.html'
};

Handlebars.registerHelper('jobStatusClass', function (displayStatus) {
    var statusClass = displayStatus === 'Job Completed' ? 'jobCompleted'
                    : displayStatus === 'Job Rejected' ? 'jobRejected'
                    : displayStatus === 'Job Failed' ? 'jobFailed'
                    : '';
    return statusClass;
});