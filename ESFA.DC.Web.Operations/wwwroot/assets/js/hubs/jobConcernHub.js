﻿import HubBase from '/assets/js/hubs/hubBase.js';

class JobConcernHub extends HubBase {

    constructor(url, jobConcernController) {
        super(url);
        this._controller = jobConcernController;
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }
}

export default JobConcernHub;