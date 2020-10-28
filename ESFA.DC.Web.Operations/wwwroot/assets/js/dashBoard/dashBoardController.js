import { getColorForPercentage, getMessageForPercentage, padLeft, sumArrayProperty, getInitialStateModel, displayConnectionState, $on } from '/assets/js/util.js';
import Hub from '/assets/js/hubs/hub.js';

class DashBoardController {
    constructor() {
        this._serviceBusStatistics = document.getElementById('serviceBusStatistics');
        this._dashboardLinks = document.querySelectorAll('.dashboardLink');

        this._data = null;
        this._queuesSystem = null;
        this._queuesTopics = null;
        this._queuesIlr = null;
        this._year = null;
        this._yearsIntitialised = false;

        this._averageTimeToday = 0;
        this._averageTimeLastHour = 0;

        this._percentageTextRangeJobProcessing = [{ value: 85, label: 'Urgent Attention!' }, { value: 60, label: 'Needs Attention' }, { value: 0, label: 'Looking Good' }];
        this._percentageTextRangeJobQueued = [{ value: 85, label: 'Urgent Attention!' }, { value: 60, label: 'Needs Attention' }, { value: 0, label: 'Looking Good' }];
        this._percentageTextRangeSubmissionsToday = [{ value: 75, label: 'Super excited!' }, { value: 50, label: 'Feeling happy' }, { value: 0, label: 'Looking Good' }];

        $on(document.getElementById('collectionYears'), 'change', (event) => {
            this._year = parseInt(event.target.value);
            this.updateProcessingInDetails(this._data.jobStats, this._year);
            this.updateDrillDownUrl(this._dashboardLinks, this._year);
        });

        $on(window, 'pageshow', () => {
            const hub = new Hub('dashBoardHub', displayConnectionState);
            this.registerHandlers(hub);
            hub.startHub();
            this.updatePage(getInitialStateModel());
        });

    }

    registerHandlers(hub) {
        hub.registerMessageHandler("ReceiveMessage", (data) => this.updatePage(data));
    }

    updatePage(data) {
        data = JSON.parse(data);
        this._data = data;
        this.updateSync();
        this.setYears(data.jobStats);
        this.updateServiceBusStats(data.serviceBusStats);
        this.updateJobStats(data.jobStats, this._year);
    }

    setYears(jobStats) {
        if (!this._yearsIntitialised) {
            this._yearsIntitialised = true;
            this._year = jobStats.collectionYears[jobStats.collectionYears.length - 1];
            const collectionYearContainer = document.getElementById("collectionYearContainer");
            
            if (jobStats.collectionYears.length < 2) {
                collectionYearContainer.style.display = "none";
                this.defaultAllDrillDownControls(jobStats.collectionYears[0]);
            } else {
                collectionYearContainer.style.display = "block";
            }
            this.addYearsToDropdown(jobStats.collectionYears);
        }
    }

    addYearsToDropdown(years) {
        const select = document.getElementById("collectionYears");
        for (let index in years) {
            select.options[select.options.length] = new Option(this.formatYearForDisplay(years[index]), years[index]);
        }
        select.selectedIndex = years.length - 1;
        this.defaultAllDrillDownControls(select.options[select.selectedIndex].value);
    }

    formatYearForDisplay(year) {
        const yearAsString = year.toString();
        if (yearAsString.length === 4) {
            return yearAsString.substr(0, 2) + "/" + yearAsString.substr(2);
        }
        return yearAsString;
    }

    updateJobStats(jobStats, year) {

        const statsForAllYears = this.getStatsForAllYears(jobStats);

        this.updateDisplay(document.getElementById("average"), statsForAllYears.averageProcessingTime);
        this.updateTrendDisplay(document.getElementById("averageSymbol"), this._averageTimeToday, statsForAllYears.averageTimeToday);
        this._averageTimeToday = statsForAllYears.averageTimeToday;

        this.updateDisplay(document.getElementById("averageLastHour"), statsForAllYears.averageProcessingTimeLastHour);
        this.updateTrendDisplay(document.getElementById("averageLastHourSymbol"), this._averageTimeLastHour, statsForAllYears.averageTimeLastHour);
        this._averageTimeLastHour = statsForAllYears.averageTimeLastHour;

        const processingPercentage = (statsForAllYears.jobsProcessing / 125) * 100;
        this.updateDonut(document.getElementById("firstDonut"), document.getElementById("firstCircle"), document.getElementById("firstLabel"), statsForAllYears.jobsProcessing, processingPercentage, this._percentageTextRangeJobProcessing);

        const jobsQueuedPercentage = (statsForAllYears.jobsQueued / 125) * 100;
        this.updateDonut(document.getElementById("secondDonut"), document.getElementById("secondCircle"), document.getElementById("secondLabel"), statsForAllYears.jobsQueued, jobsQueuedPercentage, this._percentageTextRangeJobQueued);

        let submissionsTodayPercentage = statsForAllYears.submissionsToday < 2500 ? (statsForAllYears.submissionsToday / 2500) * 100 : 100;
        this.updateDonut(document.getElementById("thirdDonut"), document.getElementById("thirdCircle"), document.getElementById("thirdLabel"), statsForAllYears.submissionsToday, submissionsTodayPercentage, this._percentageTextRangeSubmissionsToday);

        this.updateDisplay(document.getElementById("failedToday"), statsForAllYears.failedToday);
        this.updateDisplay(document.getElementById("slowFiles"), jobStats.slowFilesComparedToThreePreviousModel.slowFilesComparedToThreePrevious);
        this.updateDisplay(document.getElementById("concerns"), jobStats.concernsModel.concerns);

        this.updateProcessingInDetails(jobStats, year);
    }

    getStatsForAllYears(jobStats) {
        return {
            submissionsToday: sumArrayProperty(jobStats.todayStatsForYearModel, 'submissionsToday'),
            jobsQueued: sumArrayProperty(jobStats.todayStatsForYearModel, 'jobsQueued'),
            jobsProcessing: sumArrayProperty(jobStats.todayStatsForYearModel, 'jobsProcessing'),
            failedToday: sumArrayProperty(jobStats.todayStatsForYearModel, 'failedToday'),
            averageTimeToday: jobStats.todayProcessingTimeModel.averageTimeToday,
            averageProcessingTime: jobStats.todayProcessingTimeModel.averageProcessingTime,
            averageProcessingTimeLastHour: jobStats.todayProcessingTimeModel.averageProcessingTimeLastHour,
            averageTimeLastHour: jobStats.todayProcessingTimeModel.averageTimeLastHour
        }
    }

    updateDisplay(element, text) {
        if (element.textContent !== text.toString()) {
            element.textContent = text;
        }
    }

    getTodayStatsModelForYear(year, todayProcessingTimeModel) {
        if (!todayProcessingTimeModel) {
            return this.emptyStatsModel(year);
        }
        const todaysStats = todayProcessingTimeModel.find(s => s.collectionYear === year);
        return todaysStats ? todaysStats : this.emptyStatsModel(year);
    }

    emptyStatsModel(year) {
        return {
            "averageTimeToday": 0,
            "averageProcessingTime": "00m 00s",
            "averageTimeLastHour": 0,
            "averageProcessingTimeLastHour": "00m 00s",
            "jobsProcessing": 0,
            "jobsQueued": 0,
            "failedToday": 0,
            "submissionsToday": 0,
            "submissionsLastHour": 0,
            "submissionsLast5Minutes": 0,
            "collectionYear": year
        };
    }

    updateTrendDisplay(element, oldValue, newValue) {
        if (oldValue < newValue) {
            element.textContent = '▲';
            element.setAttribute("style", "color: red;font-size: 20px");
        }
        else if (oldValue > newValue) {
            element.textContent = '▼';
            element.setAttribute("style", "color: green;font-size: 20px");
        }
        else {
            element.textContent = '⟷';
            element.setAttribute("style", "color: orange;font-size: 20px");
        }
    }

    updateDonut(donut, circle, label, value, percentage, percentageRange) {
        if (donut.textContent !== value.toString()) {
            donut.textContent = value;
            circle.setAttribute("stroke-dasharray", `${percentage},100`);
            circle.setAttribute("style", "stroke:" + getColorForPercentage(percentage));
        }
        label.textContent = getMessageForPercentage(percentage, percentageRange);
    }


    getYearSpecificValue(array, property, year) {
        // Assumption: Only one period for a year can ever be open.
        const item = array.find(f => f.collectionYear === year);
        if (item) {
            return item[property];
        }
        return 0;
    }

    updateProcessingInDetails(jobStats, year) {
        const todaysStatsForYear = this.getTodayStatsModelForYear(year, jobStats.todayStatsForYearModel);
        this.updateDisplay(document.getElementById("lastPeriod"), this.getYearSpecificValue(jobStats.jobsCurrentPeriodModels, 'jobsCurrentPeriod', year));
        this.updateDisplay(document.getElementById("lastHour"), todaysStatsForYear.submissionsLastHour);
        this.updateDisplay(document.getElementById("last5Minutes"), todaysStatsForYear.submissionsLast5Minutes);
        this.updateDisplay(document.getElementById("ilrReturns"), this.getYearSpecificValue(jobStats.currentPeriodIlrModels, 'countOfSuccessfulIlrProvidersInPeriod', year));
        this.updateDisplay(document.getElementById("failedFiles"), this.getYearSpecificValue(jobStats.jobsCurrentPeriodModels, 'jobsFailedInPeriod', year));

        const dasPaymentDifferencesModelForYear = jobStats.dasPaymentDifferencesModels.find(f => f.collectionYear === year);
        this.updateDisplay(document.getElementById("sldDasMismatches"), dasPaymentDifferencesModelForYear ? dasPaymentDifferencesModelForYear.ukPrns.length : 0);
    }

    updateServiceBusStats(serviceBusStats) {
        if (this._serviceBusStatistics === null) {
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

        if (this._queuesSystem === null) {
            ctx = document.getElementById('queueSystem').getContext('2d');
            this._queuesSystem = this.buildChart(ctx, labelQueue, dataQueue, dataQueueDeadLetter);
        }
        else {
            this.updateChart(this._queuesSystem, labelQueue, dataQueue, dataQueueDeadLetter);
        }

        if (this._queuesTopics === null) {
            ctx = document.getElementById('queueTopics').getContext('2d');
            this._queuesTopics = this.buildChart(ctx, labelTopic, dataTopic, dataTopicDeadLetter);
        }
        else {
            this.updateChart(this._queuesTopics, labelTopic, dataTopic, dataTopicDeadLetter);
        }

        if (this._queuesIlr === null) {
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
        let day = padLeft(date.getDate(), "0", 2);
        let month = padLeft(date.getMonth() + 1, "0", 2);

        let hours = padLeft(date.getHours(), "0", 2);
        let minutes = padLeft(date.getMinutes(), "0", 2);
        let seconds = padLeft(date.getSeconds(), "0", 2);

        const dateLabel = document.getElementById("lastSync");
        dateLabel.textContent = `Last updated: ${day}/${month}/${date.getFullYear()} ${hours}:${minutes}:${seconds}`;

        const timeLabel = document.getElementById("lastTime");
        timeLabel.textContent = `${hours}:${minutes}:${seconds}`;
    }

    updateDrillDownUrl(links, year) {
        for (let index in links) {
            links.item(index).setAttribute("href", "/processing/" + links.item(index).id + "/?collectionYear=" + year);
        }
    }

    defaultAllDrillDownControls(year) {
        this.updateDrillDownUrl(this._dashboardLinks, year);
    }
}

export const dashboardController = new DashBoardController();
