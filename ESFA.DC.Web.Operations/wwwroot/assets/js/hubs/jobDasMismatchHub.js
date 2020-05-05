import HubBase from '/assets/js/hubs/hubBase.js';

class JobDasMismatchHub extends HubBase {

    constructor(url, jobDasMismatchController) {
        super(url);
        this._controller = jobDasMismatchController;
    }

    startHub() {
        super.getConnection().on("ReceiveMessage", this._controller.updatePage.bind(this._controller));
        super.startHub(this._controller);
    }
}

export default JobDasMismatchHub;