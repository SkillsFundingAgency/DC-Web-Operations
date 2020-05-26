import HubBase from '/assets/js/hubs/hubBase.js';

class JobFailedCurrentPeriodHub extends HubBase {

    constructor(url, jobFailedCurrentPeriodController) {
        super(url);
        this._controller = jobFailedCurrentPeriodController;
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }
}

export default JobFailedCurrentPeriodHub;