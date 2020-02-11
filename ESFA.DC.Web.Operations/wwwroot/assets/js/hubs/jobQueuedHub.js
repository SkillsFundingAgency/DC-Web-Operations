import JobQueuedController from '/assets/js/processing/jobQueuedController.js';

class JobQueuedHub {

    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/jobQueuedHub", { transport: signalR.HttpTransportType.WebSockets })
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub() {

        const jobQueuedController = new JobQueuedController();

        this.connection.on("ReceiveMessage", jobQueuedController.updatePage.bind(jobQueuedController));

        this.connection.onreconnecting((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            jobQueuedController.displayConnectionState("Reconnecting");
        });

        this.connection.onreconnected((connectionId) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            jobQueuedController.displayConnectionState("Connected");
        });

        this.connection.onclose((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            jobQueuedController.displayConnectionState("Closed");
        });

        this.startConnection(jobQueuedController);
    }

    startConnection(JobQueuedController) {
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
                JobQueuedController.displayConnectionState("Connected");
            });
        } catch (err) {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(), 1000);
        }
    }
}

export default JobQueuedHub;