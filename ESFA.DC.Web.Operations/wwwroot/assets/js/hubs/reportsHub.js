import ReportsController from '/assets/js/reports/reportsController.js';
import HubBase from '/assets/js/hubs/hubBase.js';

class ReportsHub extends HubBase {

    constructor(url) {
        super(url);
        this._controller = new ReportsController();
    }

    startHub() {
        super.startHub(this._controller, this._controller.getReports.bind(this._controller));
    }

}

export default ReportsHub