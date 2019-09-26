class pathController {

    pathItemCompare( a, b ) {
        if (a.ordinal < b.ordinal) {
            return -1;
        }
        if (a.ordinal > b.ordinal) {
            return 1;
        }
        return 0;
    }

    jobStatusConvertor(status) {
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

   isJobComplete(jobStatus) {
        if (jobStatus === 4)
            return true;
        return false;
    }

    renderProceed(pathItem, enabled, collectionYear, period) {
        const node =
            `<span style="display:inline-block;" >
            <form method='post' action='/periodEnd/periodEnd/proceed'>
                <input type="hidden" name="collectionYear" value="${collectionYear}" />
                <input type="hidden" name="period" value="${period}" />
                <input type="submit" value="Proceed" style="margin-left:20px;" ${enabled ? "" : "disabled"} /> 
            </form>
        </span>`;
        pathItem.insertAdjacentHTML('beforeend', node);
    }

    renderJob(job, jobList) {
        let jobItem = document.createElement("li");
        jobItem.textContent = `Job Id : ${job.jobId}, Status : ${this.jobStatusConvertor(job.status)}`;
        jobList.appendChild(jobItem);

        return this.isJobComplete(job.status);
    }

    renderPathItem(path, pathItem, subItemList, collectionYear, period) {
        let classScope = this;

        let currentItem = pathItem.ordinal === path.position - 1;

        let totalPathItems = path.pathItems.length;

        let enableProceed = true;

        let item = document.createElement("li");
        item.className += "app-task-list__item";
        item.textContent = pathItem.name;

        let jobItems = pathItem.pathItemJobs;
        if (jobItems != undefined && jobItems.length > 0) {
            let jobList = document.createElement("ul");
            jobItems.forEach(function(job) {
                let completed = classScope.renderJob(job, jobList);
                if (!completed) {
                    enableProceed = false;
                }
            });

            item.appendChild(jobList);
        }

        if ((pathItem.ordinal < path.position - 1 ||
                (pathItem.ordinal + 1 === path.position && pathItem.ordinal + 1 === totalPathItems)) &&
            (jobItems == undefined || jobItems.length === 0)) {
            item.textContent += " - Status : Completed";
        }

        if (currentItem) {
            if (pathItem.ordinal + 1 !== totalPathItems) {
                this.renderProceed(item, enableProceed, collectionYear, period);
            }

            let bold = document.createElement("b");
            subItemList.appendChild(bold);

            bold.appendChild(item);

            subItemList.appendChild(bold);
        } else {
            subItemList.appendChild(item);
        }
    }

    disableStart() {
        let startButton = document.getElementById("startPeriodEnd");
        if (startButton != null) {
            startButton.disabled = true;
        }
    }

    renderPaths(pathString) {

        if (pathString === "" || pathString === undefined) {
            return;
        }

        const paths = JSON.parse(pathString);

        let pathContainer = document.getElementById("pathContainer");
        while (pathContainer.firstChild) {
            pathContainer.removeChild(pathContainer.firstChild);
        }

        let classScope = this;
        paths.forEach(function(path) {
            //if (path.position >= 0) {
            //    disableStart();
            //}

            let period = path.period;
            let collectionYear = path.collectionYear;

            let pathItems = path.pathItems;

            if (pathItems != undefined && pathItems.length > 0) {
                pathItems.sort(classScope.pathItemCompare);

                let li = document.createElement("li");

                let title = document.createElement("h2");
                title.textContent = path.name;
                li.appendChild(title);

                let subItemList = document.createElement("ul");
                subItemList.className += "app-task-list__items";

                pathItems.forEach(function(pathItem) {
                    if (pathItem.name != undefined && pathItem.name !== "") {
                        classScope.renderPathItem(path, pathItem, subItemList, collectionYear, period);
                    }
                });

                li.appendChild(subItemList);

                pathContainer.appendChild(li);
            }
        });
    }
}

export default pathController;