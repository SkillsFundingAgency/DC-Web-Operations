import DashBoardController from '/assets/js/dashBoard/dashBoardController.js';

class dashBoardHub {

    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/dashBoardHub", { transport: signalR.HttpTransportType.WebSockets })
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub() {

        const dashBoardController = new DashBoardController();

        this.connection.on("ReceiveMessage", dashBoardController.updatePage.bind(dashBoardController));

        this.connection.onreconnecting((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            dashBoardController.displayConnectionState("Reconnecting");
        });

        this.connection.onreconnected((connectionId) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            dashBoardController.displayConnectionState("Connected");
        });

        this.connection.onclose((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            dashBoardController.displayConnectionState("Closed");
        });

        this.startConnection(dashBoardController);
    }

    startConnection(dashBoardController) {
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
                dashBoardController.displayConnectionState("Connected");
            });
        } catch (err) {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(), 1000);
        }
    }
}

export default dashBoardHub;