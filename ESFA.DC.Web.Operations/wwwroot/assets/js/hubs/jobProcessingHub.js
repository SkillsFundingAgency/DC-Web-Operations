import JobProcessingController from '/assets/js/processing/jobProcessingController.js';
import HubBase from '/assets/js/hubs/hubBase.js';

class jobProcessingHub extends HubBase{

    constructor(url) {
        super(url);
        this._controller = new JobProcessingController();
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }
}

export default jobProcessingHub;