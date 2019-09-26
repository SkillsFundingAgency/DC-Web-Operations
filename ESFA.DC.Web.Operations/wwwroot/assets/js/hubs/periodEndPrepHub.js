import JobController from '/assets/js/periodEnd/jobController.js';

class periodEndPrepHub {
    
    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Information)
            .withUrl("/periodEndPrepHub", { transport: signalR.HttpTransportType.WebSockets }) 
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub() {
        const jobController = new JobController();
        const classScope = this;

        this.connection.on("ReceiveMessage", jobController.renderJobs.bind(jobController));

        this.connection.start().then(() => {
            setTimeout(function() {
                classScope.connection.invoke('ReceiveMessage').catch(err => console.error(err.toString()));
            }, 30*1000);
        });
    }
}

export default periodEndPrepHub;