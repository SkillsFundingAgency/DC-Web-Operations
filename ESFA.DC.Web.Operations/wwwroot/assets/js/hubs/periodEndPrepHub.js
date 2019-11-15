import JobController from '/assets/js/periodEnd/jobController.js';

class periodEndPrepHub {
    
    constructor() {
        this.connection = new signalR
            .HubConnectionBuilder()
            .withUrl("/periodEndPrepHub", { transport: signalR.HttpTransportType.WebSockets }) 
            .withAutomaticReconnect()
            .build();
    }

    getConnection() {
        return this.connection;
    }

    startHub(isCurrentPeriod) {
        
        if (!isCurrentPeriod) {
            return;
        }

        const jobController = new JobController();

        this.connection.on("ReceiveMessage", jobController.renderJobs.bind(jobController));

        this.connection.on("DisableJobReSubmit",
            (jobId) => {
                jobController.disableJobReSubmit.call(jobController, jobId);
            });

        this.connection.on("ReferenceJobsButtonState",
            (enabled) => {
                jobController.setPauseRefJobsButtonState.call(jobController, enabled);
            });

        this.connection.on("CollectionClosedEmailButtonState",
            (enabled) => {
                jobController.setCollectionClosedEmailButtonState.call(jobController, enabled);
            });

        this.connection.on("ContinueButtonState",
            (enabled) => {
                jobController.setContinueButtonState.call(jobController, enabled);
            });
        
        this.connection.onreconnecting((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            jobController.displayConnectionState("Reconnecting");
        });

        this.connection.onreconnected((connectionId) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            jobController.displayConnectionState("Connected");
        });

        this.connection.onclose((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            jobController.displayConnectionState("Closed");
        });

        this.startConnection(jobController);
    }

    startConnection(jobController) {
        const classScope = this;

        try {
            this.connection.start().then(() => {
                clearTimeout(this.timerId);
                this.timerId = setTimeout(function() {
                    classScope.connection.invoke('ReceiveMessage').catch(err => console.error(err.toString()));
                }, 5*1000);
                console.assert(this.connection.state === signalR.HubConnectionState.Connected);
                console.log("connected");
                jobController.displayConnectionState("Connected");
            });
        } catch (err) {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log(err);
            setTimeout(() => this.startConnection(), 1000);
        }
    }
}

export default periodEndPrepHub;