export const getHandleBarsTemplate = function (name, root) {
    if (root === undefined) {
        root = '/assets/templates/';
    }

    if (Handlebars.templates === undefined || Handlebars.templates[name] === undefined) {
        var xmlhttp;
        if (window.XMLHttpRequest) {
            xmlhttp = new XMLHttpRequest();
        }
        else {
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
        xmlhttp.onreadystatechange = function () {
            if (xmlhttp.readyState === XMLHttpRequest.DONE) {
                if (xmlhttp.status === 200) {
                    if (Handlebars.templates === undefined) {
                        Handlebars.templates = {};
                    }
                    Handlebars.templates[name] = Handlebars.compile(xmlhttp.responseText);
                    return Handlebars.templates[name];
                }
                else {
                    console.log('Error fetching the template.');
                }
            }
        }

        xmlhttp.open("GET", root + name + '.html', true);
        xmlhttp.send();
    }
    return Handlebars.templates[name];
};

Handlebars.registerHelper('jobStatusClass', function (displayStatus) {
    var statusClass = displayStatus === 'Job Completed' ? 'jobCompleted'
                    : displayStatus === 'Job Rejected' ? 'jobRejected'
                    : displayStatus === 'Job Failed' ? 'jobFailed'
                    : '';
    return statusClass;
});