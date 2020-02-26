import JobSubmittedController from '/assets/js/processing/jobSubmittedController.js';
import HubBase from '/assets/js/hubs/hubBase.js';

class JobSubmittedHub extends HubBase {

    constructor(url) {
        super(url);
        this._controller = new JobSubmittedController();
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }

}

export default JobSubmittedHub;