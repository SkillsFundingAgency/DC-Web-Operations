import PathController from '/assets/js/periodEnd/pathController.js';

class periodEndHub {
    
    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/periodEndHub", { transport: signalR.HttpTransportType.WebSockets }) 
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub(isCurrentPeriod) {
        
        if (!isCurrentPeriod) {
            return;
        }

        const pathController = new PathController();

        this.connection.on("ReceiveMessage", pathController.renderPaths.bind(pathController));

        this.connection.on("StartPeriodEndState",
            (enabled) => { pathController.setButtonState.call(pathController, enabled, "startPeriodEnd") });

        this.connection.on("MCAReportsState",
            (enabled) => { pathController.setButtonState.call(pathController, enabled, "publishMcaReports") });

        this.connection.on("ProviderReportsState",
            (enabled) => { pathController.setButtonState.call(pathController, enabled, "publishProviderReports") });

        this.connection.on("PeriodClosedState",
            (enabled) => { pathController.setButtonState.call(pathController, enabled, "closePeriodEnd") });

        this.connection.on("ReferenceJobsButtonState",
            (enabled) => { pathController.setButtonState.call(pathController, enabled, "resumeReferenceData") });

        this.connection.on("DisablePathItemProceed",
            (pathItemId) => {
                pathController.disableProceed.call(pathController, pathItemId);
            }
        );
        
        this.connection.onreconnecting((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            pathController.displayConnectionState("Reconnecting");
        });

        this.connection.onreconnected((connectionId) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            pathController.displayConnectionState("Connected");
        });

        this.connection.onclose((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            pathController.displayConnectionState("Closed");
        });

        this.startConnection(pathController);

    }

    startConnection(pathController) {
        const classScope = this;

        try {
            this.connection.start().then(() => {
                clearTimeout(this.timerId);
                this.timerId = setInterval(function () {
                    console.log('Attempting to send client handshake as ' + classScope.connection.connectionId);
                    classScope.connection.invoke('ReceiveMessage').catch(err => console.error(err.toString()));
                }, 5*1000);
                console.assert(this.connection.state === signalR.HubConnectionState.Connected);
                console.log("connected");
                pathController.displayConnectionState("Connected");
            });
        } catch (err) {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(), 1000);
        }
    }
}

export default periodEndHub;