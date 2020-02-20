import ReportsController from '/assets/js/reports/reportsController.js';

class ReportsHub {

    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/reportsHub")
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub() {
        const controller = new ReportsController();

        this.connection.onreconnecting((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            controller.displayConnectionState("Reconnecting");
        });

        this.connection.onreconnected((connectionId) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            controller.displayConnectionState("Connected");
        });

        this.connection.onclose((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            controller.displayConnectionState("Closed");
        });

        this.startConnection(controller);
    }

    startConnection(controller) {
        try {
            this.connection.start().then(() => {
                clearTimeout(this.timerId);
                console.assert(this.connection.state === signalR.HubConnectionState.Connected);
                console.log("connected");
                controller.displayConnectionState("Connected");
                controller.getReports();
            });
        } catch (err) {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(), 1000);
        }
    }
}

export default ReportsHub