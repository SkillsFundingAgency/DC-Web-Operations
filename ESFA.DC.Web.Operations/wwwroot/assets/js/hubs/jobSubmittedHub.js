import JobSubmittedController from '/assets/js/processing/jobSubmittedController.js';

class JobSubmittedHub {

    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/jobSubmittedHub", { transport: signalR.HttpTransportType.WebSockets })
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub() {

        const jobSubmittedController = new JobSubmittedController();

        this.connection.on("ReceiveMessage", jobSubmittedController.updatePage.bind(jobSubmittedController));

        this.connection.onreconnecting((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            jobSubmittedController.displayConnectionState("Reconnecting");
        });

        this.connection.onreconnected((connectionId) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            jobSubmittedController.displayConnectionState("Connected");
        });

        this.connection.onclose((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            jobSubmittedController.displayConnectionState("Closed");
        });

        this.startConnection(jobSubmittedController);
    }

    startConnection(JobSubmittedController) {
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
                JobSubmittedController.displayConnectionState("Connected");
            });
        } catch (err) {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(), 1000);
        }
    }
}

export default JobSubmittedHub;