class Hub {
    constructor(url, displayStateDelegate) {
        this._displayStateDelegate = displayStateDelegate;

        this._connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl(`/${url}`, { transport: signalR.HttpTransportType.WebSockets })
            .build();
    }

    getConnection() {
        return this._connection;
    }

    startHub(onStartedDelegate) {
        this._connection.onreconnecting((error) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Reconnecting);
            console.log(`Reconnecting - ${error}`);
            this.updateDisplayState("Reconnecting");
        });

        this._connection.onreconnected((connectionId) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Connected);
            console.log(`Connected - ${connectionId}`);
            this.updateDisplayState("Connected");
        });

        this._connection.onclose((error) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Disconnected);
            console.log(`Closed - ${error}`);
            this.updateDisplayState("Closed");
        });

        this.startConnection(onStartedDelegate);
    }

    registerMessageHandler(message, action) {
        this._connection.on(message, action);
    }

    unregisterMessageHandler(message) {
        this._connection.off(message);
    }

    unregisterMessageHandlers(messages) {
        messages.forEeach(m => this.unregisterMessageHandler(m));
    }

    clearInterval() {
        clearInterval(this._timerId);
    }

    startConnection(onStartedDelegate) {
        try {
            this._connection.start().then(() => {
                clearTimeout(this.timerId);

                if (this._connection.methods.receivemessage !== undefined) {
                    this.timerId = setInterval(() => {
                        console.log(`Attempting to send client handshake as ${this._connection.connectionId}`);
                        this._connection.invoke('ReceiveMessage').catch(err => console.error(err.toString()));
                    }, 5 * 1000);
                }

                console.assert(this._connection.state === signalR.HubConnectionState.Connected);
                console.log("Connected");

                this.updateDisplayState("Connected");

                if (typeof onStartedDelegate === "function") {
                    onStartedDelegate();
                }
            });
        } catch (err) {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(callback), 1000);
        }
    }

    updateDisplayState(message) {
        if (typeof this._displayStateDelegate === "function") {
            this._displayStateDelegate(message);
        }
    }
}

export default Hub;