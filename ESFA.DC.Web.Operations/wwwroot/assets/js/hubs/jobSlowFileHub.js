import JobFailedTodayController from "/assets/js/processing/jobSlowFileController.js";
import HubBase from '/assets/js/hubs/hubBase.js';

class JobSlowFileHub extends HubBase {

    constructor(url) {
        super(url);
        this._controller = new JobFailedTodayController();
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }
}

export default JobSlowFileHub;