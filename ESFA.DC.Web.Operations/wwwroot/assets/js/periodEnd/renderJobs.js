function renderReferenceJobs(jobsString) {

    if (jobsString === "" || jobsString === undefined) {
        return;
    }

    let paths = JSON.parse(jobsString);

    let container = document.getElementById('referenceDataContainer');
}

function renderFailedJobs(jobsString) {

    if (jobsString === "" || jobsString === undefined) {
        return;
    }

    let paths = JSON.parse(jobsString);

    let container = document.getElementById('failedJobContainer');
}