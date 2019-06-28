"use strict";

let connection = new signalR
    .HubConnectionBuilder()
    .configureLogging(signalR.LogLevel.Information)
    .withUrl("/periodEndHub", { transport: signalR.HttpTransportType.WebSockets }) 
    .build();

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

function renderProceed(pathItem, enabled) {
    const node = 
        `<span>
            <form method='post' action='/periodEnd/proceed'>
                <input type="submit" value="Proceed" ${enabled ? "" : "disabled"}> 
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

function renderPathItem(path, pathItem, subItemList, pathItemCount){
    let currentItem = pathItem.ordinal === path.position;

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
        if (pathItem.ordinal !== pathItemCount - 1) {
            renderProceed(item, enableProceed);
        }

        let bold = document.createElement("b");
        subItemList.appendChild(bold);

        bold.appendChild(item);

        subItemList.appendChild(bold);
    } else {
        subItemList.appendChild(item);
    }
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
        let pathItems = path.pathItems;

        let pathItemCount = pathItems.length;
        if (pathItems != undefined && pathItems.length > 0) {
            pathItems.sort(pathItemCompare);

            let li = document.createElement("li");
            li.textContent = path.name;

            let subItemList = document.createElement("ul");
            pathItems.forEach(function(pathItem) {
                renderPathItem(path, pathItem, subItemList, pathItemCount);
            });

            li.appendChild(subItemList);

            pathContainer.appendChild(li);
        }
    });
}

connection.on("ReceiveMessage", renderPaths);

connection.start();