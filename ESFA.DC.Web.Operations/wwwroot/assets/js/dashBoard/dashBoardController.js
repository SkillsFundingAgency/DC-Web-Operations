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
        this._lastPeriod = document.getElementById("lastPeriod");
        this._lastHour = document.getElementById("lastHour");
        this._last5Minutes = document.getElementById("last5Minutes");

        this._queuesSystem = null;
        this._queuesTopics = null;
        this._queuesIlr = null;

        this._percentColors = [
            { pct: 1, color: { r: 0x00, g: 0xff, b: 0 } },
            { pct: 50, color: { r: 0xff, g: 0xff, b: 0 } },    
            { pct: 100, color: { r: 0xff, g: 0x00, b: 0 } }
        ];
    }

    updatePage(data) {
        this.updateSync();
        this.updateServiceBusStats(data.serviceBusStats);
        this.updateJobStats(data.jobStats);
    }

    // https://stackoverflow.com/a/7128796
    getColorForPercentage(pct) {
        var i = 1;
        for (; i < this._percentColors.length - 1; i++) {
            if (pct < this._percentColors[i].pct) {
                break;
            }
        }

        var lower = this._percentColors[i - 1];
        var upper = this._percentColors[i];
        var range = upper.pct - lower.pct;
        var rangePct = (pct - lower.pct) / range;
        var pctLower = 1 - rangePct;
        var pctUpper = rangePct;
        var color = {
            r: Math.floor(lower.color.r * pctLower + upper.color.r * pctUpper),
            g: Math.floor(lower.color.g * pctLower + upper.color.g * pctUpper),
            b: Math.floor(lower.color.b * pctLower + upper.color.b * pctUpper)
        };
        return 'rgb(' + [color.r, color.g, color.b].join(',') + ')';
        // or output as hex if preferred
    } 

    updateJobStats(jobStats) {
        if (this._averageLabel.textContent !== jobStats.todayStatsModel.averageProcessingTime) {
            this._averageLabel.textContent = `${jobStats.todayStatsModel.averageProcessingTime}`;
        }

        if (this._firstDonut.textContent !== jobStats.todayStatsModel.jobsProcessing.toString()) {
            this._firstDonut.textContent = `${jobStats.todayStatsModel.jobsProcessing}`;
            this._firstCircle.setAttribute("stroke-dasharray", `${jobStats.todayStatsModel.jobsProcessing},125`);
            this._firstCircle.setAttribute("style", "stroke:" + this.getColorForPercentage((jobStats.todayStatsModel.jobsProcessing / 125) * 100));
        }

        if (this._secondDonut.textContent !== jobStats.todayStatsModel.jobsQueued.toString()) {
            this._secondDonut.textContent = `${jobStats.todayStatsModel.jobsQueued}`;
            this._secondCircle.setAttribute("stroke-dasharray", `${jobStats.todayStatsModel.jobsQueued},100`);
            this._secondCircle.setAttribute("style", "stroke:" + this.getColorForPercentage((jobStats.todayStatsModel.jobsQueued / 100) * 100));
        }

        if (this._thirdDonut.textContent !== jobStats.todayStatsModel.submissionsToday.toString()) {
            this._thirdDonut.textContent = `${jobStats.todayStatsModel.submissionsToday}`;
            let value = 2500;
            if (jobStats.todayStatsModel.submissionsToday < 2500) {
                value = (jobStats.todayStatsModel.submissionsToday / 2500) * 100;
            }

            this._thirdCircle.setAttribute("stroke-dasharray", `${value},100`);
            this._thirdCircle.setAttribute("style", "stroke:" + this.getColorForPercentage(value));
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
                today += "/" + jobStats.jobsCurrentPeriodModels[i].PeriodNumber.toString();

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
    }

    updateServiceBusStats(serviceBusStats) {
        let labelQueue = [], labelTopic = [], labelIlr = [];
        let dataQueue = [], dataTopic = [], dataIlr = [];
        let dataQueueDeadLetter = [], dataTopicDeadLetter = [], dataIlrDeadLetter = [];
        let lenQueue = serviceBusStats.queues.length, lenTopics = serviceBusStats.topics.length, lenIlr = serviceBusStats.ilr.length;
        let i = 0;

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
            var ctx = document.getElementById('queueSystem').getContext('2d');
            this._queuesSystem = new Chart(ctx,
                {
                    type: 'bar',
                    data: {
                        labels: labelQueue,
                        datasets: [
                            {
                                label: '# of Messages',
                                data: dataQueue,
                                backgroundColor: 'rgb(0,255,0)',
                                borderWidth: 1
                            },
                            {
                                label: '# of Dead Letters',
                                data: dataQueueDeadLetter,
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
        else
        {
            this._queuesSystem.data.labels = labelQueue;
            this._queuesSystem.data.datasets[0].data = dataQueue;
            this._queuesSystem.data.datasets[1].data = dataQueueDeadLetter;
            this._queuesSystem.update();
        }

        if (this._queuesTopics == null) {
            var ctx = document.getElementById('queueTopics').getContext('2d');
            this._queuesTopics = new Chart(ctx,
                {
                    type: 'bar',
                    data: {
                        labels: labelTopic,
                        datasets: [
                            {
                                label: '# of Messages',
                                data: dataTopic,
                                backgroundColor: 'rgb(0,255,0)',
                                borderWidth: 1
                            },
                            {
                                label: '# of Dead Letters',
                                data: dataTopicDeadLetter,
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
        else
        {
            this._queuesTopics.data.labels = labelTopic;
            this._queuesTopics.data.datasets[0].data = dataTopic;
            this._queuesTopics.data.datasets[1].data = dataTopicDeadLetter;
            this._queuesTopics.update();
        }

        if (this._queuesIlr == null) {
            var ctx = document.getElementById('queueIlr').getContext('2d');
            this._queuesIlr = new Chart(ctx,
                {
                    type: 'bar',
                    data: {
                        labels: labelIlr,
                        datasets: [
                            {
                                label: '# of Messages',
                                data: dataIlr,
                                backgroundColor: 'rgb(0,255,0)',
                                borderWidth: 1
                            },
                            {
                                label: '# of Dead Letters',
                                data: dataIlrDeadLetter,
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
        else {
            this._queuesIlr.data.labels = labelIlr;
            this._queuesIlr.data.datasets[0].data = dataIlr;
            this._queuesIlr.data.datasets[1].data = dataIlrDeadLetter;
            this._queuesIlr.update();
        }
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