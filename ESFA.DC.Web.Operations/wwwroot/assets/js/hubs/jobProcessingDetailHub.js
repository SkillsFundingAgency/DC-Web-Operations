import HubBase from '/assets/js/hubs/hubBase.js';

class jobProcessingDetailHub extends HubBase{

    constructor(url, jobProcessingController) {
        super(url);
        this._controller = jobProcessingController;
    }

    startHub() {
        super.startHub(this._controller);
    }
}

export default jobProcessingDetailHub;