import HubBase from '/assets/js/hubs/hubBase.js';

class JobQueuedHub extends HubBase {

    constructor(url, jobQueuedController) {
        super(url);
        this._controller = jobQueuedController;
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }

}

export default JobQueuedHub;