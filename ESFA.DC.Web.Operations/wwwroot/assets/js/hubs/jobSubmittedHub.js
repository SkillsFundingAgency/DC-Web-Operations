import HubBase from '/assets/js/hubs/hubBase.js';

class JobSubmittedHub extends HubBase {

    constructor(url, jobSubmittedController) {
        super(url);
        this._controller = jobSubmittedController;
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }

}

export default JobSubmittedHub;