import HubBase from '/assets/js/hubs/hubBase.js';

class jobProcessingDetailHub extends HubBase{

    constructor(url, jobProcessingController) {
        super(url);
        this._controller = jobProcessingController;
    }

    startHub() {
        //super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
        //super.startHub(this._controller);
    }
}

export default jobProcessingDetailHub;