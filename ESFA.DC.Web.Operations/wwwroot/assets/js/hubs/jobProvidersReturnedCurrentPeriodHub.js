import HubBase from '/assets/js/hubs/hubBase.js';

class JobProvidersReturnedCurrentPeriodHub extends HubBase {

    constructor(url, jobProvidersReturnedCurrentPeriodController) {
        super(url);
        this._controller = jobProvidersReturnedCurrentPeriodController;
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }
}

export default JobProvidersReturnedCurrentPeriodHub;