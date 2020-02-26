class HubBase {
    constructor(url, controller) {

        this._controller;
        this._callback;

        this._connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl(`/${url}`, { transport: signalR.HttpTransportType.WebSockets })
            .build();

    }

    getConnection() {

        return this._connection;

    }

    startHub(controller, callback) {

        this._controller = controller;
        this._callback = callback;

        this._connection.onreconnecting((error) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            this._controller.displayConnectionState("Reconnecting");
        });

        this._connection.onreconnected((connectionId) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            this._controller.displayConnectionState("Connected");
        });

        this._connection.onclose((error) => {
            console.assert(this._connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);

            this._controller.displayConnectionState("Closed");
        });

        this.startConnection();

    }

    startConnection() {

        const classScope = this;

        try {
            this._connection.start().then(() => {

                clearTimeout(this.timerId);

                if (classScope._connection.methods.receivemessage !== undefined) {

                    this.timerId = setInterval(function () {
                        console.log('Attempting to send client handshake as ' + classScope._connection.connectionId);
                        classScope._connection.invoke('ReceiveMessage').catch(err => console.error(err.toString()));
                    }, 5 * 1000);

                }

                console.assert(this._connection.state === signalR.HubConnectionState.Connected);

                console.log("connected");

                this._controller.displayConnectionState("Connected");

                if (this._callback !== undefined && typeof this._callback == "function") {
                    this._callback();
                }

            });
        } catch (err) {
            console.assert(this._connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(), 1000);
        }

    }
}

export default HubBase;