import JobFailedTodayController from '/assets/js/processing/jobFailedTodayController.js';

class JobFailedTodayHub {

    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/jobFailedTodayHub", { transport: signalR.HttpTransportType.WebSockets })
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub() {

        const jobFailedTodayController = new JobFailedTodayController();

        this.connection.on("ReceiveMessage", jobFailedTodayController.updatePage.bind(jobFailedTodayController));

        this.connection.onreconnecting((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            jobFailedTodayController.displayConnectionState("Reconnecting");
        });

        this.connection.onreconnected((connectionId) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            jobFailedTodayController.displayConnectionState("Connected");
        });

        this.connection.onclose((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            jobFailedTodayController.displayConnectionState("Closed");
        });

        this.startConnection(jobFailedTodayController);
    }

    startConnection(JobFailedTodayController) {
        const classScope = this;

        try {
            this.connection.start().then(() => {
                clearTimeout(this.timerId);
                this.timerId = setInterval(function () {
                    console.log('Attempting to send client handshake as ' + classScope.connection.connectionId);
                    classScope.connection.invoke('ReceiveMessage').catch(err => console.error(err.toString()));
                }, 5 * 1000);
                console.assert(this.connection.state === signalR.HubConnectionState.Connected);
                console.log("connected");
                JobFailedTodayController.displayConnectionState("Connected");
            });
        } catch (err) {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(), 1000);
        }
    }
}

export default JobFailedTodayHub;