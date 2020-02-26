import JobQueuedController from '/assets/js/processing/jobQueuedController.js';
import HubBase from '/assets/js/hubs/hubBase.js';

class JobQueuedHub extends HubBase {

    constructor(url) {
        super(url);
        this._controller = new JobQueuedController();
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }

}

export default JobQueuedHub;