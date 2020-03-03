import HubBase from '/assets/js/hubs/hubBase.js';

class JobSlowFileHub extends HubBase {

    constructor(url, jobFailedTodayController) {
        super(url);
        this._controller = jobFailedTodayController;
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }
}

export default JobSlowFileHub;