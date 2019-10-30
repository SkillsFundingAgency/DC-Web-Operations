import ProviderSearchController from '/assets/js/providerSearch/providerSearchController.js';

class providerSearchHub {

    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .withAutomaticReconnect()
            .withUrl("/providerSearchHub", { transport: signalR.HttpTransportType.WebSockets })
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub() {
        const controller = new ProviderSearchController();

        this.connection.on("updateSearchResults", controller.updateSearchResults.bind(controller));
        this.startConnection(controller);
    }

    startConnection(controller) {
        try {
            this.connection.start();
        } catch (err) {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
        }
    }
}

export default providerSearchHub