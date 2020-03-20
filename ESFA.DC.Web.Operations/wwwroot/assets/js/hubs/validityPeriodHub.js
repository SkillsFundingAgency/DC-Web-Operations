import HubBase from '/assets/js/hubs/hubBase.js';

class ValidityPeriodHub extends HubBase {

    constructor(url, validityPeriodHubController) {
        super(url);
        this._controller = validityPeriodHubController;
    }

    getConnection() {
        return super.getConnection();
    }

    startHub(callback) {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.getConnection().on("GetValidityPeriodList", this._controller.updatePage.bind(this._controller));
        super.getConnection().on("UpdateValidityPeriod", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller, callback);
    }

}

export default ValidityPeriodHub;