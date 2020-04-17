import JobController from '/assets/js/periodEnd/ncs/jobController.js';
import HubBase from '/assets/js/hubs/hubBase.js';

class periodEndPrepHub extends HubBase {

    constructor(url) {
        super(url);
        this._controller = new JobController();
    }

    startHub() {

        this.connection = super.getConnection();
        
        this.connection.on("ReceiveMessage", this._controller.renderJobs.bind(this._controller));

        this.connection.on("DisableJobReSubmit",
            (jobId) => {
                this._controller.disableJobReSubmit.call(this._controller, jobId);
            });

        this.connection.on("CollectionClosedEmailButtonState",
            (enabled) => {
                this._controller.setCollectionClosedEmailButtonState.call(this._controller, enabled);
            });

        this.connection.on("ContinueButtonState",
            (enabled) => {
                this._controller.setContinueButtonState.call(this._controller, enabled);
            });

        this.connection.onreconnecting((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Reconnecting);
            console.log("Reconnecting - " + error);
            this._controller.displayConnectionState("Reconnecting");
        });

        this.connection.onreconnected((connectionId) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Connected);
            console.log("Connected - " + connectionId);
            this._controller.displayConnectionState("Connected");
        });

        this.connection.onclose((error) => {
            console.assert(this.connection.state === signalR.HubConnectionState.Disconnected);
            console.log("Closed - " + error);
            this._controller.displayConnectionState("Closed");
        });

        super.startHub(this._controller);
    }
    
}

export default periodEndPrepHub;