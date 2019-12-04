class DashBoardController {
    constructor() {
        this._averageLabel = document.getElementById("average");
        this._firstDonut = document.getElementById("firstDonut");
        this._firstCircle = document.getElementById("firstCircle");
        this._secondDonut = document.getElementById("secondDonut");
        this._secondCircle = document.getElementById("secondCircle");
        this._thirdDonut = document.getElementById("thirdDonut");
        this._thirdCircle = document.getElementById("thirdCircle");
        this._failedToday = document.getElementById("failedToday");
        this._slowFiles = document.getElementById("slowFiles");
        this._concerns = document.getElementById("concerns");
    }

    updatePage(data) {
        this.updateSync();
        this.updateServiceBusStats(data.serviceBusStats);
        this.updateJobStats(data.jobStats);
    }

    updateJobStats(jobStats) {
        if (this._averageLabel.textContent !== jobStats.averageProcessingTime.toString()) {
            this._averageLabel.textContent = `${jobStats.averageProcessingTime}`;
        }

        if (this._firstDonut.textContent !== jobStats.jobsProcessing.toString()) {
            this._firstDonut.textContent = `${jobStats.jobsProcessing}`;
            this._firstCircle.setAttribute("stroke-dasharray", `${jobStats.jobsProcessing},125`);
        }

        if (this._secondDonut.textContent !== jobStats.jobsQueued.toString()) {
            this._secondDonut.textContent = `${jobStats.jobsQueued}`;
            this._secondCircle.setAttribute("stroke-dasharray", `${jobStats.jobsQueued},100`);
        }

        if (this._thirdDonut.textContent !== jobStats.submissions.toString()) {
            this._thirdDonut.textContent = `${jobStats.submissions}`;
            let value = 2500;
            if (jobStats.submissions < 2500) {
                value = (jobStats.submissions / 2500) * 100;
            }

            this._thirdCircle.setAttribute("stroke-dasharray", `${value},100`);
        }

        if (this._failedToday.textContent !== jobStats.failedToday.toString()) {
            this._failedToday.textContent = `${jobStats.failedToday}`;
        }

        if (this._slowFiles.textContent !== jobStats.slowFiles.toString()) {
            this._slowFiles.textContent = `${jobStats.slowFiles}`;
        }

        if (this._concerns.textContent !== jobStats.concerns.toString()) {
            this._concerns.textContent = `${jobStats.concerns}`;
        }
    }

    updateServiceBusStats(serviceBusStats) {

    }

    updateSync() {
        let date = new Date();
        let day = this.padLeft(date.getDate(), "0", 2);
        let month = this.padLeft(date.getMonth() + 1, "0", 2);

        let hours = this.padLeft(date.getHours(), "0", 2);
        let minutes = this.padLeft(date.getMinutes(), "0", 2);
        let seconds = this.padLeft(date.getSeconds(), "0", 2);

        const dateLabel = document.getElementById("lastSync");
        dateLabel.textContent = `Last updated: ${day}/${month}/${date.getFullYear()} ${hours}:${minutes}:${seconds}`;

        const timeLabel = document.getElementById("lastTime");
        timeLabel.textContent = `${hours}:${minutes}:${seconds}`;
    }

    displayConnectionState(state) {
        const stateLabel = document.getElementById("state");
        stateLabel.textContent = `Status: ${state}`;
    }

    padLeft(str, padString, max) {
        str = str.toString();
        return str.length < max ? this.padLeft(padString + str, padString, max) : str;
    }
}

export default DashBoardController