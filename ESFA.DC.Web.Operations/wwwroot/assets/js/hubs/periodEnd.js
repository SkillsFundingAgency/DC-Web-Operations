"use strict";

var connection = new signalR
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
    case 1:
        return "Moved For Processing";
    case 1:
        return "Processing";
    case 1:
        return "Completed";
    case 1:
        return "Failed Retry";
    case 1:
        return "Failed";
    case 1:
        return "Paused";
    case 1:
        return "Waiting";
    default:
    }
}

function renderProceed(pathItem) {
    const node = 
        `<span>
            <form method='post' action='/periodend/proceed'>
                <input type="submit" value="Proceed"> 
            </form>
        </span>`;
    pathItem.insertAdjacentHTML('beforeend', node);
}

function renderJob(job, jobList) {
    let jobItem = document.createElement("li");
    jobItem.textContent = `Job Id : ${job.jobId}, Status : ${jobStatusConvertor(job.status)}`;
    jobList.appendChild(jobItem);
}

function renderPathItem(path, pathItem, subItemList){
    let currentItem = pathItem.ordinal === path.position;
    
    let item = document.createElement("li");
    item.textContent = pathItem.name;

    let jobItems = pathItem.pathItemJobs;
    if (jobItems != undefined && jobItems.length > 0) {
        var jobList = document.createElement("ul");
        jobItems.forEach(function(job) {
            renderJob(job, jobList);
        });

        item.appendChild(jobList);
    }

    if (currentItem) {
        renderProceed(item);
    
        let bold = document.createElement("b");
        subItemList.appendChild(bold);

        bold.appendChild(item);

        subItemList.appendChild(bold);
    } else {
        subItemList.appendChild(item);
    }
}


connection.on("ReceiveMessage",
    function(pathString) {
        
        let paths = JSON.parse(pathString);

        let pathContainer = document.getElementById("pathContainer");
        while (pathContainer.firstChild) {
            pathContainer.removeChild(pathContainer.firstChild);
        }

        paths.forEach(function(path) {
            let pathItems = path.pathItems;

            if (pathItems != undefined && pathItems.length > 0) {
                pathItems.sort(pathItemCompare);

                let li = document.createElement("li");
                li.textContent = path.name;

                let subItemList = document.createElement("ul");
                pathItems.forEach(function(pathItem) {
                    renderPathItem(path, pathItem, subItemList);
                });

                li.appendChild(subItemList);

                pathContainer.appendChild(li);
            }
        });
});

connection.start();