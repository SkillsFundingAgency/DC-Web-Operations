
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

connection.on("ReceiveMessage",
    function(pathString) {
        
        var paths = JSON.parse(pathString);

        var pathContainer = document.getElementById("pathContainer");
        while (pathContainer.firstChild) {
            pathContainer.removeChild(pathContainer.firstChild);
        }

        paths.forEach(function(path) {
            var pathItems = path.pathItems;

            if (pathItems != undefined && pathItems.length > 0) {
                pathItems.sort(pathItemCompare);

                var li = document.createElement("li");
                li.textContent = path.name;

                var subItemList = document.createElement("ol");
                pathItems.forEach(function(pathItem) {
                    var currentItem = pathItem.ordinal === path.position;
                    var bold = document.createElement("b");

                    if (currentItem) {
                        subItemList.appendChild(bold);
                    }

                    var item = document.createElement("li");
                    item.textContent = pathItem.name;

                    var jobItems = pathItem.pathItemJobs;
                    if (jobItems != undefined && jobItems.length > 0) {
                        var jobList = document.createElement("ol");
                        jobItems.forEach(function(job) {
                            var jobItem = document.createElement("li");
                            jobItem.textContent = job.jobId;
                            jobList.appendChild(jobItem);
                        });

                        item.appendChild(jobList);
                    }

                    if (currentItem) {
                        bold.appendChild(item);
                        subItemList.appendChild(bold);
                    } else {
                        subItemList.appendChild(item);
                    }
                });

                li.appendChild(subItemList);

                pathContainer.appendChild(li);
            }
        });


});

connection.start();