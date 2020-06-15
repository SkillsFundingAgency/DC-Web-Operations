class referenceDataHub {
    
    constructor(referenceDataController) {
        this._timerId = 0;

        this._referenceDataController = referenceDataController;
        this._connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/referenceDataHub", { transport: signalR.HttpTransportType.WebSockets }) 
            .build();
    }

    getConnection() {
        return this._connection;
    }

    startHub() {
        this._connection.on("ReceiveMessage", this._referenceDataController.renderFiles.bind(this._referenceDataController));

        this._connection.onreconnecting((error) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            this._referenceDataController.displayConnectionState("Reconnecting");
        });

        this._connection.onreconnected((connectionId) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            this._referenceDataController.displayConnectionState("Connected");
        });

        this._connection.onclose((error) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            this._referenceDataController.displayConnectionState("Closed");
        });

        this.startConnection(null);
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
                classScope._referenceDataController.displayConnectionState(message);
            });
        } catch (err) {
            console.assert(this._connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(delegate), 1000);
        }
    }
}

export default referenceDataHub;