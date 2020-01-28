class periodEndHub {
    
    constructor(pathController) {
        this._pathController = pathController;
        this._connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/periodEndHub", { transport: signalR.HttpTransportType.WebSockets }) 
            .build();
    }

    getConnection() {
        return this._connection;
    }

    startHub() {
        this._connection.on("ReceiveMessage", this._pathController.renderPaths.bind(this._pathController));

        this._connection.on("StartPeriodEndState",
            (enabled) => { this._pathController.setButtonState.call(this._pathController, enabled, "startPeriodEnd") });

        this._connection.on("MCAReportsState",
            (enabled) => { this._pathController.setButtonState.call(this._pathController, enabled, "publishMcaReports") });

        this._connection.on("ProviderReportsState",
            (enabled) => { this._pathController.setButtonState.call(this._pathController, enabled, "publishProviderReports") });

        this._connection.on("PeriodClosedState",
            (enabled) => { this._pathController.setButtonState.call(this._pathController, enabled, "closePeriodEnd") });

        this._connection.on("ReferenceJobsButtonState",
            (enabled) => { this._pathController.setButtonState.call(this._pathController, enabled, "resumeReferenceData") });

        this._connection.on("DisablePathItemProceed",
            (pathItemId) => {
                this._pathController.disableProceed.call(this._pathController, pathItemId);
            }
        );
        
        this._connection.onreconnecting((error) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            this._pathController.displayConnectionState("Reconnecting");
        });

        this._connection.onreconnected((connectionId) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            this._pathController.displayConnectionState("Connected");
        });

        this._connection.onclose((error) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            this._pathController.displayConnectionState("Closed");
        });

        this.startConnection();

    }

    startConnection() {
        const classScope = this;

        try {
            this._connection.start().then(() => {
                clearTimeout(this.timerId);
                this.timerId = setInterval(function () {
                    console.log('Attempting to send client handshake as ' + classScope._connection.connectionId);
                    classScope._connection.invoke('ReceiveMessage').catch(err => console.error(err.toString()));
                }, 5*1000);
                console.assert(this._connection.state === signalR.HubConnectionState.Connected);
                console.log("connected");
                classScope._pathController.displayConnectionState("Connected");
            });
        } catch (err) {
            console.assert(this._connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(), 1000);
        }
    }
}

export default periodEndHub;