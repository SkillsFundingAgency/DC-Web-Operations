//let startButton = document.getElementById("startPeriodEnd");
//startButton.addEventListener("click", startPeriodEnd);


function pathItemCompare( a, b ) {
    if ( a.ordinal < b.ordinal ){
        return -1;
    }
    if ( a.ordinal > b.ordinal ){
        return 1;
    }
    return 0;
}

function jobStatusConvertor(status) {
    switch (status) {
        case 1:
            return "Ready";
        case 2:
            return "Moved For Processing";
        case 3:
            return "Processing";
        case 4:
            return "Completed";
        case 5:
            return "Failed Retry";
        case 6:
            return "Failed";
        case 7:
            return "Paused";
        case 8:
            return "Waiting";
        default:
    }
}

function isJobComplete(jobStatus) {
    if (jobStatus === 4)
        return true;
    return false;
}

function renderProceed(pathItem, enabled, collectionYear, period) {
    const node = 
        `<span style="display:inline-block;" >
            <form method='post' action='/periodEnd/proceed'>
                <input type="hidden" name="collectionYear" value="${collectionYear}" />
                <input type="hidden" name="period" value="${period}" />
                <input type="submit" value="Proceed" style="margin-left:20px;" ${enabled ? "" : "disabled"} /> 
            </form>
        </span>`;
    pathItem.insertAdjacentHTML('beforeend', node);
}

function renderJob(job, jobList) {
    let jobItem = document.createElement("li");
    jobItem.textContent = `Job Id : ${job.jobId}, Status : ${jobStatusConvertor(job.status)}`;
    jobList.appendChild(jobItem);

    return isJobComplete(job.status);
}

function renderPathItem(path, pathItem, subItemList, collectionYear, period){
    let currentItem = pathItem.ordinal === path.position -1;

    let enableProceed = true;

    let item = document.createElement("li");
    item.textContent = pathItem.name;

    let jobItems = pathItem.pathItemJobs;
    if (jobItems != undefined && jobItems.length > 0) {
        var jobList = document.createElement("ul");
        jobItems.forEach(function(job) {
            var completed  = renderJob(job, jobList);
            if (!completed) {
                enableProceed = false;
            }
        });

        item.appendChild(jobList);
    }

    if (currentItem) {
        renderProceed(item, enableProceed, collectionYear, period);
        
        let bold = document.createElement("b");
        subItemList.appendChild(bold);

        bold.appendChild(item);

        subItemList.appendChild(bold);
    } else {
        subItemList.appendChild(item);
    }
}

function disableStart() {
    let startButton = document.getElementById("startPeriodEnd");
    if(startButton != null) {
        startButton.disabled = true;
    }
}

function startPeriodEnd(collectionYear, period) {
    connection
        .invoke("StartPeriodEnd", collectionYear, period)
        .catch(err => console.error(err.toString()));
}

function renderPaths(pathString) {

    if (pathString === "" || pathString === undefined) {
        return;
    }

    let paths = JSON.parse(pathString);

    let pathContainer = document.getElementById("pathContainer");
    while (pathContainer.firstChild) {
        pathContainer.removeChild(pathContainer.firstChild);
    }

    paths.forEach(function(path) {
        //if (path.position >= 0) {
        //    disableStart();
        //}

        let period = path.period;
        let collectionYear = path.collectionYear;

        let pathItems = path.pathItems;

        if (pathItems != undefined && pathItems.length > 0) {
            pathItems.sort(pathItemCompare);

            let li = document.createElement("li");
            li.textContent = path.name;

            let subItemList = document.createElement("ul");
            pathItems.forEach(function(pathItem) {
                renderPathItem(path, pathItem, subItemList, collectionYear, period);
            });

            li.appendChild(subItemList);

            pathContainer.appendChild(li);
        }
    });
}