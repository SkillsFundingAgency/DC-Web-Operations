class periodEndHub {
    
    constructor(pathController) {
        this._timerId = 0;

        this._pathController = pathController;
        this._connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/referenceDataHub", { transport: signalR.HttpTransportType.WebSockets }) 
            .build();
    }

    getConnection() {
        return this._connection;
    }

    startHub(stateModel) {
        if (!stateModel.isPreviousPeriod) {
            this._connection.on("ReceiveMessage", this._pathController.renderFiles.bind(this._pathController));
        }

        this._connection.on("UploadState",
            (enabled) => { this._pathController.setButtonState.call(this._pathController, enabled, "uploadFile") });

        this._connection.on("TurnOffMessage", () => {
            this._connection.off("UploadState");
            this._connection.off("ReceiveMessage");

            clearInterval(this._timerId);
        });

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

        this.startConnection(this.startTimer);
    }

    startTimer() {
        const scope = this;
        this._timerId =  setInterval(function () {
            console.log('Attempting to send client handshake as ' + scope._connection.connectionId);
            scope._connection.invoke('ReceiveMessage').catch(err => console.error(err.toString()));
        }, 5*1000);
    }

    startConnection(delegate) {
        const classScope = this;

        try {
            this._connection.start().then(() => {
                if (delegate) {
                    delegate.call(classScope);
                }
                
                console.assert(this._connection.state === signalR.HubConnectionState.Connected);
                var message = delegate ? "Connected" : "Connected minimal";
                console.log(message);
                classScope._pathController.displayConnectionState(message);
            });
        } catch (err) {
            console.assert(this._connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(delegate), 1000);
        }
    }
}

export default periodEndHub;