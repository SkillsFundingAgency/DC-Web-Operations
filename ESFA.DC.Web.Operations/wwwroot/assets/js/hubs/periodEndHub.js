import PathController from '/assets/js/periodEnd/pathController.js';

class periodEndHub {
    
    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl("/periodEndHub", { transport: signalR.HttpTransportType.WebSockets }) 
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub() {
        const pathController = new PathController();
        const classScope = this;

        this.connection.on("ReceiveMessage", pathController.renderPaths.bind(pathController));

        this.connection.on("DisableStartPeriodEnd", pathController.disableStart.bind(pathController));

        this.connection.start().then(() => {
            setTimeout(function() {
                classScope.connection.invoke("ReceiveMessage").catch(err => console.error(err.toString()));
            }, 30 * 1000);
        });
    }
}

export default periodEndHub;