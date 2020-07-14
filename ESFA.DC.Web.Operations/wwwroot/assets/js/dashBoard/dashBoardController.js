import { getColorForPercentage } from '/assets/js/util.js';
import { getMessageForPercentage } from '/assets/js/util.js';

class DashBoardController {
    constructor() {
        this._averageLabel = document.getElementById("average");
        this._averageSymbolLabel = document.getElementById("averageSymbol");

        this._averageLastHourLabel = document.getElementById("averageLastHour");
        this._averageLastHourSymbolLabel = document.getElementById("averageLastHourSymbol");

        this._firstDonut = document.getElementById("firstDonut");
        this._firstCircle = document.getElementById("firstCircle");
        this._firstLabel = document.getElementById("firstLabel");
        this._secondDonut = document.getElementById("secondDonut");
        this._secondCircle = document.getElementById("secondCircle");
        this._secondLabel = document.getElementById("secondLabel");
        this._thirdDonut = document.getElementById("thirdDonut");
        this._thirdCircle = document.getElementById("thirdCircle");
        this._thirdLabel = document.getElementById("thirdLabel");
        this._failedToday = document.getElementById("failedToday");
        this._slowFiles = document.getElementById("slowFiles");
        this._concerns = document.getElementById("concerns");
        this._lastPeriod = document.getElementById("lastPeriod");
        this._lastHour = document.getElementById("lastHour");
        this._last5Minutes = document.getElementById("last5Minutes");
        this._ilrReturns = document.getElementById("ilrReturns");
        this._failedFiles = document.getElementById("failedFiles");
        this._sldDasMismatches = document.getElementById("sldDasMismatches");
        this._serviceBusStatistics = document.getElementById('serviceBusStatistics');

        this._queuesSystem = null;
        this._queuesTopics = null;
        this._queuesIlr = null;

        this._averageTimeToday = 0;
        this._averageTimeLastHour = 0;

        this._percentageTextRangeJobProcessing = [{ value: 85, label: 'Urgent Attention!' }, { value: 60, label: 'Needs Attention' }, { value: 0, label: 'Looking Good' }];
        this._percentageTextRangeJobQueued = [{ value: 85, label: 'Urgent Attention!' }, { value: 60, label: 'Needs Attention' }, { value: 0, label: 'Looking Good' }];
        this._percentageTextRangeSubmissionsToday = [{ value: 75, label: 'Super excited!' }, { value: 50, label: 'Feeling happy' }, { value: 0, label: 'Looking Good' }];
    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
    }

    updatePage(data) {
        data = JSON.parse(data);

        this.updateSync();
        this.updateServiceBusStats(data.serviceBusStats);
        this.updateJobStats(data.jobStats);
    }

    updateJobStats(jobStats) {
        if (this._averageLabel.textContent !== jobStats.todayStatsModel.averageProcessingTime) {
            this._averageLabel.textContent = `${jobStats.todayStatsModel.averageProcessingTime}`;
        }

        if (this._averageTimeToday < jobStats.todayStatsModel.averageTimeToday) {
            this._averageSymbolLabel.textContent = '▲';
            this._averageSymbolLabel.setAttribute("style", "color: red;font-size: 20px");
        }
        else if (this._averageTimeToday > jobStats.todayStatsModel.averageTimeToday) {
            this._averageSymbolLabel.textContent = '▼';
            this._averageSymbolLabel.setAttribute("style", "color: green;font-size: 20px");
        }
        else {
            this._averageSymbolLabel.textContent = '⟷';
            this._averageSymbolLabel.setAttribute("style", "color: orange;font-size: 20px");
        }

        this._averageTimeToday = jobStats.todayStatsModel.averageTimeToday;


        if (this._averageLastHourLabel.textContent !== jobStats.todayStatsModel.averageProcessingTimeLastHour) {
            this._averageLastHourLabel.textContent = `${jobStats.todayStatsModel.averageProcessingTimeLastHour}`;
        }

        if (this._averageTimeLastHour < jobStats.todayStatsModel.averageTimeLastHour) {
            this._averageLastHourSymbolLabel.textContent = '▲';
            this._averageLastHourSymbolLabel.setAttribute("style", "color: red;font-size: 20px");
        }
        else if (this._averageTimeLastHour > jobStats.todayStatsModel.averageTimeLastHour) {
            this._averageLastHourSymbolLabel.textContent = '▼';
            this._averageLastHourSymbolLabel.setAttribute("style", "color: green;font-size: 20px");
        }
        else {
            this._averageLastHourSymbolLabel.textContent = '⟷';
            this._averageLastHourSymbolLabel.setAttribute("style", "color: orange;font-size: 20px");
        }

        this._averageTimeLastHour = jobStats.todayStatsModel.averageTimeLastHour;

        if (this._firstDonut.textContent !== jobStats.todayStatsModel.jobsProcessing.toString()) {
            this._firstDonut.textContent = `${jobStats.todayStatsModel.jobsProcessing}`;
            let percentage = (jobStats.todayStatsModel.jobsProcessing / 125) * 100;
            this._firstCircle.setAttribute("stroke-dasharray", `${percentage},100`);
            this._firstCircle.setAttribute("style", "stroke:" + getColorForPercentage(percentage));
            this._firstLabel.textContent = getMessageForPercentage(percentage, this._percentageTextRangeJobProcessing);
        }

        if (this._secondDonut.textContent !== jobStats.todayStatsModel.jobsQueued.toString()) {
            this._secondDonut.textContent = `${jobStats.todayStatsModel.jobsQueued}`;
            this._secondCircle.setAttribute("stroke-dasharray", `${jobStats.todayStatsModel.jobsQueued},100`);
            let percentage = (jobStats.todayStatsModel.jobsQueued / 125) * 100;
            this._secondCircle.setAttribute("style", "stroke:" + getColorForPercentage(percentage));
            this._secondLabel.textContent = getMessageForPercentage(percentage, this._percentageTextRangeJobQueued);
        }

        if (this._thirdDonut.textContent !== jobStats.todayStatsModel.submissionsToday.toString()) {
            this._thirdDonut.textContent = `${jobStats.todayStatsModel.submissionsToday}`;
            let percentage = 100;
            if (jobStats.todayStatsModel.submissionsToday < 2500) {
                percentage = (jobStats.todayStatsModel.submissionsToday / 2500) * 100;
            }

            this._thirdCircle.setAttribute("stroke-dasharray", `${percentage},100`);
            this._thirdCircle.setAttribute("style", "stroke:" + getColorForPercentage(percentage));
            this._thirdLabel.textContent = getMessageForPercentage(percentage, this._percentageTextRangeSubmissionsToday);
        }

        if (this._failedToday.textContent !== jobStats.todayStatsModel.failedToday.toString()) {
            this._failedToday.textContent = `${jobStats.todayStatsModel.failedToday}`;
        }

        if (this._slowFiles.textContent !== jobStats.slowFilesComparedToThreePreviousModel.slowFilesComparedToThreePrevious.toString()) {
            this._slowFiles.textContent = `${jobStats.slowFilesComparedToThreePreviousModel.slowFilesComparedToThreePrevious}`;
        }

        if (this._concerns.textContent !== jobStats.concernsModel.concerns.toString()) {
            this._concerns.textContent = `${jobStats.concernsModel.concerns}`;
        }

        let today = "";
        let len = jobStats.jobsCurrentPeriodModels.length;
        for (var i = 0; i < len; i++) {
            today += jobStats.jobsCurrentPeriodModels[i].jobsCurrentPeriod;

            if (len > 1) {
                today += "/" + jobStats.jobsCurrentPeriodModels[i].periodNumber.toString();

                if (i < len - 1) {
                    today += " - ";
                }
            }
        }

        if (today === "") {
            today = "0";
        }

        if (this._lastPeriod.textContent !== today) {
            this._lastPeriod.textContent = `${today}`;
        }

        if (this._lastHour.textContent !== jobStats.todayStatsModel.submissionsLastHour.toString()) {
            this._lastHour.textContent = `${jobStats.todayStatsModel.submissionsLastHour}`;
        }

        if (this._last5Minutes.textContent !== jobStats.todayStatsModel.submissionsLast5Minutes.toString()) {
            this._last5Minutes.textContent = `${jobStats.todayStatsModel.submissionsLast5Minutes}`;
        }

        let ilrsReturned = "";
        len = jobStats.currentPeriodIlrModels.length;
        for (var i = 0; i < len; i++) {
            ilrsReturned += jobStats.currentPeriodIlrModels[i].countOfSuccessfulIlrProvidersInPeriod;

            if (len > 1) {
                ilrsReturned += "/" + jobStats.currentPeriodIlrModels[i].periodNumber.toString();

                if (i < len - 1) {
                    ilrsReturned += " - ";
                }
            }
        }

        if (this._ilrReturns.textContent !== ilrsReturned) {
            this._ilrReturns.textContent = `${ilrsReturned}`;
        }

        let failedFiles = "";
        len = jobStats.jobsCurrentPeriodModels.length;
        for (var i = 0; i < len; i++) {
            failedFiles += jobStats.jobsCurrentPeriodModels[i].jobsFailedInPeriod;

            if (len > 1) {
                failedFiles += "/" + jobStats.jobsCurrentPeriodModels[i].periodNumber.toString();

                if (i < len - 1) {
                    failedFiles += " - ";
                }
            }
        }

        if (this._failedFiles.textContent !== failedFiles) {
            this._failedFiles.textContent = `${failedFiles}`;
        }

        let sldDasMismatches = jobStats.dasPaymentDifferencesModels.length;

        if (this._sldDasMismatches.textContent !== sldDasMismatches) {
            this._sldDasMismatches.textContent = `${sldDasMismatches}`;
        }
    }

    updateServiceBusStats(serviceBusStats) {
        if (this._serviceBusStatistics == null) {
            return;
        }

        let labelQueue = [], labelTopic = [], labelIlr = [];
        let dataQueue = [], dataTopic = [], dataIlr = [];
        let dataQueueDeadLetter = [], dataTopicDeadLetter = [], dataIlrDeadLetter = [];
        let lenQueue = serviceBusStats.queues.length, lenTopics = serviceBusStats.topics.length, lenIlr = serviceBusStats.ilr.length;
        let i = 0, ctx = null;

        for (i = 0; i < lenQueue; i++) {
            labelQueue.push(serviceBusStats.queues[i].name);
            dataQueue.push(serviceBusStats.queues[i].messageCount);
            dataQueueDeadLetter.push(serviceBusStats.queues[i].deadLetterMessageCount);
        }

        for (i = 0; i < lenTopics; i++) {
            labelTopic.push(serviceBusStats.topics[i].name);
            dataTopic.push(serviceBusStats.topics[i].messageCount);
            dataTopicDeadLetter.push(serviceBusStats.topics[i].deadLetterMessageCount);
        }

        for (i = 0; i < lenIlr; i++) {
            labelIlr.push(serviceBusStats.ilr[i].name);
            dataIlr.push(serviceBusStats.ilr[i].messageCount);
            dataIlrDeadLetter.push(serviceBusStats.ilr[i].deadLetterMessageCount);
        }

        if (this._queuesSystem == null) {
            ctx = document.getElementById('queueSystem').getContext('2d');
            this._queuesSystem = this.buildChart(ctx, labelQueue, dataQueue, dataQueueDeadLetter);
        }
        else {
            this.updateChart(this._queuesSystem, labelQueue, dataQueue, dataQueueDeadLetter);
        }

        if (this._queuesTopics == null) {
            ctx = document.getElementById('queueTopics').getContext('2d');
            this._queuesTopics = this.buildChart(ctx, labelTopic, dataTopic, dataTopicDeadLetter);
        }
        else {
            this.updateChart(this._queuesTopics, labelTopic, dataTopic, dataTopicDeadLetter);
        }

        if (this._queuesIlr == null) {
            ctx = document.getElementById('queueIlr').getContext('2d');
            this._queuesIlr = this.buildChart(ctx, labelIlr, dataIlr, dataIlrDeadLetter);
        } else {
            this.updateChart(this._queuesIlr, labelIlr, dataIlr, dataIlrDeadLetter);
        }
    }

    buildChart(ctx, labels, messages, deadLetters) {
        return new Chart(ctx,
            {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: '# of Messages',
                            data: messages,
                            backgroundColor: 'rgb(0,255,0)',
                            borderWidth: 1
                        },
                        {
                            label: '# of Dead Letters',
                            data: deadLetters,
                            backgroundColor: 'rgb(255,-12240,0)',
                            borderWidth: 1
                        }
                    ]
                },
                options: {
                    scales: {
                        xAxes: [
                            {
                                stacked: true,
                                afterFit: (scale) => {
                                    scale.height = 75;
                                }
                            }
                        ],
                        yAxes: [
                            {
                                stacked: true,
                                ticks: {
                                    beginAtZero: true
                                }
                            }
                        ]
                    }
                }
            });
    }

    updateChart(chart, labels, messages, deadLetters) {
        chart.data.labels = labels;
        chart.data.datasets[0].data = messages;
        chart.data.datasets[1].data = deadLetters;
        chart.update();
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